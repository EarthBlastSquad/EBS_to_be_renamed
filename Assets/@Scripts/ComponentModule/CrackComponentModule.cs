namespace ComponentModule
{
    public class CrackComponentModule
    {
        private int _stageIdx = -1;
        private float _stageStep = 0;
        private float _nextStageThreshold = 0;
        public void Init(int maxHP)
        {
            _stageStep = maxHP / 5f;
            _nextStageThreshold = maxHP - _stageStep;
            _stageIdx = -1;
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