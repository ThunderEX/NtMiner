﻿using NTMiner.Gpus;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : OverClockBase, IOverClock {
        private readonly AdlHelper _adlHelper;
        public AMDOverClock(AdlHelper adlHelper) {
            _adlHelper = adlHelper;
        }

        public void SetCoreClock(int gpuIndex, int value, int voltage) {
            base.SetCoreClock(gpuIndex, value, voltage, _adlHelper.SetCoreClock);
        }

        public void SetMemoryClock(int gpuIndex, int value, int voltage) {
            base.SetMemoryClock(gpuIndex, value, voltage, _adlHelper.SetMemoryClock);
        }

        public void SetPowerLimit(int gpuIndex, int value) {
            base.SetPowerLimit(gpuIndex, value, _adlHelper.SetPowerLimit);
        }

        public void SetTempLimit(int gpuIndex, int value) {
            base.SetTempLimit(gpuIndex, value, _adlHelper.SetTempLimit);
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            base.SetFanSpeed(gpuIndex, value, isAutoMode: value == 0, setFanSpeed: _adlHelper.SetFanSpeed);
        }

        public new void RefreshGpuState(int gpuIndex) {
            base.RefreshGpuState(gpuIndex);
        }

        public void Restore() {
            SetCoreClock(NTMinerRoot.GpuAllId, 0, 0);
            SetMemoryClock(NTMinerRoot.GpuAllId, 0, 0);
            SetPowerLimit(NTMinerRoot.GpuAllId, 0);
            SetTempLimit(NTMinerRoot.GpuAllId, 0);
            SetFanSpeed(NTMinerRoot.GpuAllId, 0);
            RefreshGpuState(NTMinerRoot.GpuAllId);
        }

        protected override void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                gpu.PowerCapacity = _adlHelper.GetPowerLimit(gpu.Index);
                gpu.TempLimit = _adlHelper.GetTempLimit(gpu.Index);
                if (_adlHelper.GetMemoryClock(gpu.Index, out int memoryClock, out int iVddc)) {
                    gpu.MemoryClockDelta = memoryClock;
                    gpu.MemoryVoltage = iVddc;
                }
                if (_adlHelper.GetCoreClock(gpu.Index, out int coreClock, out iVddc)) {
                    gpu.CoreClockDelta = coreClock;
                    gpu.CoreVoltage = iVddc;
                }
                _adlHelper.GetClockRange(
                    gpu.Index,
                    out int coreClockDeltaMin, out int coreClockDeltaMax,
                    out int memoryClockDeltaMin, out int memoryClockDeltaMax,
                    out int voltMin, out int voltMax, out int voltDefault,
                    out int powerMin, out int powerMax, out int powerDefault,
                    out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault,
                    out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault);
                gpu.CoreClockDeltaMin = coreClockDeltaMin;
                gpu.CoreClockDeltaMax = coreClockDeltaMax;
                gpu.MemoryClockDeltaMin = memoryClockDeltaMin;
                gpu.MemoryClockDeltaMax = memoryClockDeltaMax;
                gpu.PowerMin = powerMin;
                gpu.PowerMax = powerMax;
                gpu.PowerDefault = powerDefault;
                gpu.TempLimitMin = tempLimitMin;
                gpu.TempLimitMax = tempLimitMax;
                gpu.TempLimitDefault = tempLimitDefault;
                gpu.CoolMin = fanSpeedMin;
                gpu.CoolMax = fanSpeedMax;
                gpu.VoltMin = voltMin;
                gpu.VoltMax = voltMax;
                gpu.VoltDefault = voltDefault;
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
