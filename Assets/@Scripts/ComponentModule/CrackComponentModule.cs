namespace ComponentModule
{
    public class CrackComponentModule
    {
        private int _stageIdx = 0;
        private int _stageStep = 0;
        private int _nextStageThreshold = 0;
        public void Init(int maxHP)
        {
            _stageStep = maxHP / 6;
            _nextStageThreshold = maxHP - _stageStep;
            _stageIdx = 0;
        }

        public bool UpdateStage(int nowHP)
        {
            if(nowHP <= _nextStageThreshold)
            {
                _nextStageThreshold -= _stageStep;
                _stageIdx++;
                return true;
            }
            return false;
        }

        public int GetNowStage()
        {
            return _stageIdx;
        }
    }
}