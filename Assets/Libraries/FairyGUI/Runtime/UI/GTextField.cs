using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FairyGUI.Utils;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class GTextField : GObject, ITextColorGear
    {
        public TextField textField => _textField;

        protected TextField _textField;
        protected string _text;
        protected bool _ubbEnabled;
        protected bool _updatingSize;
        protected Dictionary<string, string> _templateVars;

        public GTextField()
            : base()
        {
            TextFormat tf = _textField.textFormat;
            tf.font = UIConfig.defaultFont;
            tf.size = 12;
            tf.color = Color.black;
            tf.lineSpacing = 3;
            tf.letterSpacing = 0;
            _textField.textFormat = tf;

            _text = string.Empty;
            _textField.autoSize = AutoSizeType.Both;
            _textField.wordWrap = false;
        }

        override protected void CreateDisplayObject()
        {
            _textField = new TextField();
            _textField.gOwner = this;
            displayObject = _textField;
        }

        /// <summary>
        /// 
        /// </summary>
        override public string text
        {
            get
            {
                if (this is GTextInput)
                    _text = ((GTextInput)this).inputTextField.text;
                return _text;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                _text = value;
                SetTextFieldText();
                UpdateSize();
                UpdateGear(6);
            }
        }

        virtual protected void SetTextFieldText()
        {
            string str = _text;
            if (EnableTemplateVar())
                str = ParseTemplate(str);

            _textField.maxWidth = maxWidth;
            if (_ubbEnabled)
                _textField.htmlText = UBBParser.inst.Parse(XMLUtils.EncodeString(str));
            else
                _textField.text = str;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> templateVars
        {
            get { return _templateVars; }
            set
            {
                if (_templateVars == null && value == null)
                    return;

                _templateVars = value;

                FlushVars();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public GTextField SetVar(string name, string value)
        {
            if (_templateVars == null)
                _templateVars = new Dictionary<string, string>();
            _templateVars[name] = value;

            return this;
        }
        
        public void ClearVars()
        {
            if (_templateVars == null)
                return;
            
            _templateVars.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FlushVars()
        {
            SetTextFieldText();
            UpdateSize();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasCharacter(char ch)
        {
            return _textField.HasCharacter(ch);
        }

        protected bool EnableTemplateVar()
        {
            return _templateVars != null;
        }

        protected string ParseTemplate(string template)
        {
            return StringUtils.ParseTemplate(template, _templateVars);
        }

        /// <summary>
        /// 
        /// </summary>
        public TextFormat textFormat
        {
            get
            {
                return _textField.textFormat;
            }
            set
            {
                _textField.textFormat = value;
                if (!underConstruct)
                    UpdateSize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Color color
        {
            get
            {
                return _textField.textFormat.color;
            }
            set
            {
                if (_textField.textFormat.color != value)
                {
                    TextFormat tf = _textField.textFormat;
                    tf.color = value;
                    _textField.textFormat = tf;
                    UpdateGear(4);
                }
            }
        }

        /// <summary>
        /// 渐变色
        /// </summary>
        public Color32[] gradientColor
        {
            get => _textField.textFormat.gradientColor;
            set
            {
                var tf = _textField.textFormat;
                
                if (value == null)
                {
                    if (tf.gradientColor == null)
                        return;
                    
                    tf.gradientColor = null;
                }
                else if (tf.gradientColor == value)
                {
                    // do nothing
                }
                else if (tf.gradientColor == null)
                {
                    tf.gradientColor = new Color32[4];
                    value.CopyTo(this.gradientColor, 0);
                }
                else
                {
                    value.CopyTo(this.gradientColor, 0);
                }
                
                _textField.textFormat = tf;
                UpdateGear(4);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AlignType align
        {
            get { return _textField.align; }
            set { _textField.align = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertAlignType verticalAlign
        {
            get { return _textField.verticalAlign; }
            set { _textField.verticalAlign = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool singleLine
        {
            get { return _textField.singleLine; }
            set { _textField.singleLine = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float stroke
        {
            get { return _textField.stroke; }
            set { _textField.stroke = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color strokeColor
        {
            get { return _textField.strokeColor; }
            set
            {
                _textField.strokeColor = value;
                UpdateGear(4);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 shadowOffset
        {
            get { return _textField.shadowOffset; }
            set { _textField.shadowOffset = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UBBEnabled
        {
            get { return _ubbEnabled; }
            set { _ubbEnabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AutoSizeType autoSize
        {
            get { return _textField.autoSize; }
            set
            {
                _textField.autoSize = value;
                if (value == AutoSizeType.Both)
                {
                    _textField.wordWrap = false;

                    if (!underConstruct)
                        this.SetSize(_textField.textWidth, _textField.textHeight);
                }
                else
                {
                    _textField.wordWrap = true;

                    if (value == AutoSizeType.Height)
                    {
                        if (!underConstruct)
                        {
                            displayObject.width = this.width;
                            this.height = _textField.textHeight;
                        }
                    }
                    else
                        displayObject.SetSize(this.width, this.height);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float textWidth
        {
            get { return _textField.textWidth; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float textHeight
        {
            get { return _textField.textHeight; }
        }

        protected void UpdateSize()
        {
            if (_updatingSize)
                return;

            _updatingSize = true;

            if (_textField.autoSize == AutoSizeType.Both)
            {
                this.size = displayObject.size;
                InvalidateBatchingState();
            }
            else if (_textField.autoSize == AutoSizeType.Height)
            {
                this.height = displayObject.height;
                InvalidateBatchingState();
            }

            _updatingSize = false;
        }

        override protected void HandleSizeChanged()
        {
            if (_updatingSize)
                return;

            if (underConstruct)
                displayObject.SetSize(this.width, this.height);
            else if (_textField.autoSize != AutoSizeType.Both)
            {
                if (_textField.autoSize == AutoSizeType.Height)
                {
                    displayObject.width = this.width;//先调整宽度，让文本重排
                    if (_text != string.Empty) //文本为空时，1是本来就不需要调整， 2是为了防止改掉文本为空时的默认高度，造成关联错误
                        SetSizeDirectly(this.width, displayObject.height);
                }
                else
                    displayObject.SetSize(this.width, this.height);
            }
        }

        override public void Setup_BeforeAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_BeforeAdd(buffer, beginPos);

            buffer.Seek(beginPos, 5);

            TextFormat tf = _textField.textFormat;

            tf.font = buffer.ReadS();
            tf.size = buffer.ReadShort();
            tf.color = buffer.ReadColor();
            this.align = (AlignType)buffer.ReadByte();
            this.verticalAlign = (VertAlignType)buffer.ReadByte();
            tf.lineSpacing = buffer.ReadShort();
            tf.letterSpacing = buffer.ReadShort();
            _ubbEnabled = buffer.ReadBool();
            this.autoSize = (AutoSizeType)buffer.ReadByte();
            tf.underline = buffer.ReadBool();
            tf.italic = buffer.ReadBool();
            tf.bold = buffer.ReadBool();
            this.singleLine = buffer.ReadBool();
            if (buffer.ReadBool())
            {
                tf.outlineColor = buffer.ReadColor();
                tf.outline = buffer.ReadFloat();
            }

            if (buffer.ReadBool())
            {
                tf.shadowColor = buffer.ReadColor();
                float f1 = buffer.ReadFloat();
                float f2 = buffer.ReadFloat();
                tf.shadowOffset = new Vector2(f1, f2);
            }

            if (buffer.ReadBool())
                _templateVars = new Dictionary<string, string>();

            if (buffer.version >= 3)
            {
                tf.strikethrough = buffer.ReadBool();
#if FAIRYGUI_TMPRO
                tf.faceDilate = buffer.ReadFloat();
                tf.outlineSoftness = buffer.ReadFloat();
                tf.underlaySoftness = buffer.ReadFloat();
#else
                buffer.Skip(12);
#endif
            }

            _textField.textFormat = tf;
        }

        override public void Setup_AfterAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_AfterAdd(buffer, beginPos);

            buffer.Seek(beginPos, 6);

            string str = GetCustomLocalizedText(buffer.ReadS());
            if (str != null)
                this.text = str;
            
            UIUtils.SetupGradientText(this, customData);
        }
        
        /// <summary>
        /// 设置四方向渐变色
        /// </summary>
        public void UpdateGradientColor(Color32 leftTop, Color32 leftBottom, Color32 rightTop, Color32 rightBottom)
        {
            var tf = _textField.textFormat;

            var buffer = tf.gradientColor ??= new Color32[4];
            buffer[0] = leftTop;
            buffer[1] = leftBottom;
            buffer[2] = rightTop;
            buffer[3] = rightBottom;
            
            this.gradientColor = buffer;
        }
        
        /// <summary>
        /// 设置从上到下的渐变色
        /// </summary>
        public void UpdateVerticalGradientColor(Color32 top, Color32 bottom)
        {
            UpdateGradientColor(top, bottom, top, bottom);
        }
        
        /// <summary>
        /// 设置从左到右的渐变色
        /// </summary>
        public void UpdateHorizontalGradientColor(Color32 left, Color32 right)
        {
            UpdateGradientColor(left, left, right, right);
        }
    }
}
