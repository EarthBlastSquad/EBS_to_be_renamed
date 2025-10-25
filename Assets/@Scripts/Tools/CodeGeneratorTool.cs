using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class CodeGeneratorTool
    {
        private StringBuilder _generatedCode;
        private bool _preservePlaceHolder = true;
        private Dictionary<string, string> _templates = new Dictionary<string, string>();//key : key, value : template


        #region header
        private const string _fileHeader = "CODE_GENERATOR_HEADER";
        private const string _templateTypeKeyword = "template_type";
        private const string _regionSeperator = "__[REG_SEP]__";
        #endregion

        public CodeGeneratorTool(int initialStringCapacity, bool preservePlaceHolder)
        {
            if (initialStringCapacity < 1)
            {
                initialStringCapacity = 1;
            }
            _generatedCode = new StringBuilder(initialStringCapacity);
            _preservePlaceHolder = preservePlaceHolder;
        }

        public bool IsItCorrectTemplate(string template)
        {
            return !string.IsNullOrEmpty(template) && template.Length > _fileHeader.Length && template.StartsWith(_fileHeader, System.StringComparison.Ordinal);
        }

        

    }
}
/*
구조:
첫 두줄:템플릿 정보
1. 템플릿인지에 대한 여부
2. 어떤 타입 템플릿인지에 대한 정보
그 이후로는 키와 값으로 나뉨
각 문자열들을 __[REG_SEP]__을 기준으로 청크단위로 분리되며, 공백도 있는 그대로 들어간다
그래서, 각 청크 사이에는 불필요한 공백은 넣지 않는것이 좋다
*/
/*
CODE_GENERATOR_HEADER
UI
__[REG_SEP]__
button_func_template:
__[REG_SEP]__
public void On__[NAME]__Clicked(PointerEventData data)
{

}
__[REG_SEP]__
test_temp:
__[REG_SEP]__
public __[RET_TYPE]__ __[NAME]__(__[ARGS]__)
{

}
*/