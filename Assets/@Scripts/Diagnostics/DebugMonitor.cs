using Unity.Profiling;
using UnityEngine;
using System.Text;

namespace Diagnostics
{
    public class DebugMonitor : MonoBehaviour
    {
        // 1. 메모리 관련 지표
        private ProfilerRecorder _totalUsedMemoryRecorder;
        private ProfilerRecorder _gcAllocatedInFrameRecorder;
        private ProfilerRecorder _objectCountRecorder;

        // 2. 렌더링 관련 지표
        private ProfilerRecorder _batchesRecorder;
        private ProfilerRecorder _setPassCallsRecorder;
        private ProfilerRecorder _trianglesRecorder;

        private bool _isRunning;
        private readonly Rect _displayRect = new Rect(10, 10, 300, 20); // 기본 위치 및 크기

        private StringBuilder _stringBuilder = new StringBuilder(512);

        public void ShowDiagnostics()
        {
            if (_isRunning)
            {
                return;
            }

            // 1. 메모리 레코더 설정
            _totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
            _gcAllocatedInFrameRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
            _objectCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Object Count");

            // 2. 렌더링 레코더 설정
            _batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            _setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            _trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");

            _isRunning = true;
        }

        public void StopShowDiagnostics()
        {
            if (_isRunning == false)
            {
                return;
            }

            _totalUsedMemoryRecorder.Dispose();
            _gcAllocatedInFrameRecorder.Dispose();
            _objectCountRecorder.Dispose();
            _batchesRecorder.Dispose();
            _setPassCallsRecorder.Dispose();
            _trianglesRecorder.Dispose();

            _isRunning = false;
        }

        private void Update()
        {
            // F2를 눌러 진단 화면을 켜고 끄는 토글 기능
            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (_isRunning)
                {
                    StopShowDiagnostics();
                }
                else
                {
                    ShowDiagnostics();
                }
            }
        }

        void OnGUI()
        {
            if (_isRunning == false)
            {
                return;
            }

            _stringBuilder.Clear();

            // --- 1. FPS---
            float fps = 1.0f / Time.deltaTime;
            AppendToGUI($"FPS: {fps}");

            // --- 2. 메모리 지표 ---

            // Total Used Memory (MB)
            if (_totalUsedMemoryRecorder.Valid)
            {
                float usedMemoryMB = _totalUsedMemoryRecorder.LastValue / (1024f * 1024f);
                AppendToGUI($"Total Used Memory (MB): {usedMemoryMB}");
            }

            // GC Allocated In Frame (KB)
            if (_gcAllocatedInFrameRecorder.Valid)
            {
                float gcAllocKB = _gcAllocatedInFrameRecorder.LastValue / 1024f;
                AppendToGUI($"GC Alloc In Frame (KB): {gcAllocKB}");
            }

            // Object Count
            if (_objectCountRecorder.Valid)
            {
                AppendToGUI($"Object Count: {_objectCountRecorder.LastValue}");
            }

            // --- 3. 렌더링 지표 ---

            // Triangles
            if (_trianglesRecorder.Valid)
            {
                AppendToGUI($"Triangles: {_trianglesRecorder.LastValue}");
            }

            if (_batchesRecorder.Valid)
            {
                AppendToGUI($"Batches: {_batchesRecorder.LastValue}");
            }

            // SetPass Calls
            if (_setPassCallsRecorder.Valid)
            {
                AppendToGUI($"SetPass Calls: {_setPassCallsRecorder.LastValue}");
            }

            // --- 최종 출력 ---
            GUI.Box(new Rect(_displayRect.x - 5, _displayRect.y - 5, _displayRect.width, 130), "");
            GUI.Label(new Rect(_displayRect.x, _displayRect.y, _displayRect.width, 130), _stringBuilder.ToString());
        }

        // GUI 출력을 돕는 내부 함수
        private void AppendToGUI(string text)
        {
            // \n을 추가하여 각 지표를 새로운 줄에 표시합니다.
            _stringBuilder.AppendLine(text);
        }

        void OnDisable()
        {
            // 컴포넌트 비활성화 또는 게임 종료 시 확실하게 Dispose 합니다.
            if (_isRunning)
            {
                StopShowDiagnostics();
            }
        }
    }
}
//디버그 코드는 어떤 요소를 출력할지만 지시하고, ai를 사용해 gui틀과 반복적으로 나오는 profiler를 뽑음