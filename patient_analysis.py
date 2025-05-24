import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from scipy import stats
import os

# File path
file_path = r"C:\Users\seyit\Downloads\signals_hospital.csv"

if not os.path.exists(file_path):
    print(f"Error: File not found at {file_path}")
else:
    try:
        # Read the CSV file
        df = pd.read_csv(file_path)
        
        # Convert DateTime to proper datetime format
        df['DateTime'] = pd.to_datetime(df['DateTime'])
        
        # Filter relevant signals
        signals = ['HF', 'Resp R', 'SpO2', 'FiO2(read)', 'Thuid']
        df_signals = df[df['Abbreviation'].isin(signals)].copy()
        
        # Convert Value to numeric where possible
        df_signals['Value_num'] = pd.to_numeric(df_signals['Value'], errors='coerce')
        
        # Clean data: remove artifacts (0 or 1) for specified signals
        for sig in ['HF', 'Resp R', 'SpO2']:
            mask = (df_signals['Abbreviation'] == sig)
            df_signals = df_signals[~((mask) & (df_signals['Value_num'].isin([0, 1])))]
        
        # Convert Thuid from Kelvin to Celsius
        mask_thuid = (df_signals['Abbreviation'] == 'Thuid')
        df_signals.loc[mask_thuid, 'Value_num'] = df_signals.loc[mask_thuid, 'Value_num'] - 273.15
        
        # Feature engineering per patient
        features = []
        for patient in df_signals['Code'].unique():
            patient_data = df_signals[df_signals['Code'] == patient]
            feats = {'Patient': patient}
            
            # 1. HF Variability Feature (using interquartile range)
            hf = patient_data[patient_data['Abbreviation'] == 'HF']['Value_num'].dropna()
            if not hf.empty:
                feats['HF_IQR'] = stats.iqr(hf)  # More robust than range or std
            else:
                feats['HF_IQR'] = np.nan
            
            # 2. SpO2 Critical Events (% time below 90% - more critical threshold)
            spo2 = patient_data[patient_data['Abbreviation'] == 'SpO2']['Value_num'].dropna()
            if not spo2.empty:
                feats['SpO2_Critical'] = (spo2 < 90).mean() * 100  # Percentage of critical SpO2 readings
            else:
                feats['SpO2_Critical'] = np.nan
            
            # 3. Respiratory Rate Stability
            resp = patient_data[patient_data['Abbreviation'] == 'Resp R']['Value_num'].dropna()
            if not resp.empty:
                # Calculate percentage of readings within normal range (12-20 breaths/min)
                feats['Resp_Stability'] = ((resp >= 12) & (resp <= 20)).mean() * 100
            else:
                feats['Resp_Stability'] = np.nan
            
            # 4. Oxygen Requirement (FiO2 feature)
            fio2 = patient_data[patient_data['Abbreviation'] == 'FiO2(read)']['Value_num'].dropna()
            if not fio2.empty:
                # Calculate average oxygen requirement above room air (21%)
                feats['FiO2_Requirement'] = np.mean(fio2[fio2 > 21] - 21)
            else:
                feats['FiO2_Requirement'] = np.nan
            
            # 5. Temperature Stability
            thuid = patient_data[patient_data['Abbreviation'] == 'Thuid']['Value_num'].dropna()
            if not thuid.empty:
                # Calculate percentage of readings outside normal range (36.5-37.5°C)
                feats['Temp_Instability'] = ((thuid < 36.5) | (thuid > 37.5)).mean() * 100
            else:
                feats['Temp_Instability'] = np.nan
            
            # 6. HF-SpO2 Correlation (physiological coupling)
            if not hf.empty and not spo2.empty:
                hf_df = patient_data[patient_data['Abbreviation'] == 'HF'][['DateTime', 'Value_num']]
                spo2_df = patient_data[patient_data['Abbreviation'] == 'SpO2'][['DateTime', 'Value_num']]
                merged = pd.merge_asof(hf_df.sort_values('DateTime'), 
                                     spo2_df.sort_values('DateTime'),
                                     on='DateTime',
                                     tolerance=pd.Timedelta('5min'))
                if not merged.empty:
                    feats['HF_SpO2_Corr'] = merged['Value_num_x'].corr(merged['Value_num_y'])
                else:
                    feats['HF_SpO2_Corr'] = np.nan
            else:
                feats['HF_SpO2_Corr'] = np.nan
                
            features.append(feats)
            
        # Create features dataframe
        features_df = pd.DataFrame(features)
        print("\nFeatures per patient:")
        print(features_df)
        
        # Visualization
        plt.figure(figsize=(15, 10))
        
        # Plot time series for HF
        for patient in df_signals['Code'].unique():
            patient_data = df_signals[df_signals['Code'] == patient]
            hf_data = patient_data[patient_data['Abbreviation'] == 'HF']
            plt.plot(hf_data['DateTime'], hf_data['Value_num'], 
                    label=f'HF Patient {patient}')
        
        plt.title('Hartslag over tijd')
        plt.xlabel('Tijd')
        plt.ylabel('Hartslag (HF)')
        plt.grid(True)
        plt.legend()
        plt.tight_layout()
        plt.savefig('heart_rate_time_series.png')
        plt.show()
        
        # Feature comparison visualization
        features_to_plot = features_df.drop('Patient', axis=1).columns
        features_plot_df = features_df.set_index('Patient')[features_to_plot]
        
        plt.figure(figsize=(12, 6))
        features_plot_df.plot(kind='bar')
        plt.title('Patient Features Comparison')
        plt.xlabel('Patient')
        plt.ylabel('Feature Value')
        plt.xticks(rotation=0)
        plt.legend(bbox_to_anchor=(1.05, 1), loc='upper left')
        plt.tight_layout()
        plt.savefig('features_comparison.png')
        plt.show()
        
    except Exception as e:
        print(f"Error: {str(e)}") 