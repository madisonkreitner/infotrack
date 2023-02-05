using System.Net.NetworkInformation;

namespace Googler.Models
{
    public enum TagType
    {
        COMMENT,
        DOCTYPE,
        A,
        ABBR,
        ADDRESS,
        AREA,
        ARTICLE,
        ASIDE,
        AUDIO,
        B,
        BASE,
        BDI,
        BDO,
        BLOCKQUOTE,
        BODY,
        BR,
        BUTTON,
        CANVAS,
        CAPTION,
        CITE,
        CODE,
        COL,
        COLGROUP,
        DATA,
        DATALIST,
        DD,
        DEL,
        DETAILS,
        DFN,
        DIALOG,
        DIV,
        DL,
        DT,
        EM,
        EMBED,
        FIELDSET,
        FIGCAPTION,
        FIGURE,
        FOOTER,
        FORM,
        H1,    
        H2,    
        H3,    
        H4,    
        H5,    
        H6,
        HEAD,
        HEADER,
        HR,
        HTML,
        I,
        IFRAME,
        IMG,
        INPUT,
        INS,
        KBD,
        KEYGEN,
        LABEL,
        LEGEND,
        LI,
        LINK,
        MAIN,
        MAP,
        MARK,
        META,
        METER,
        NAV,
        NOSCRIPT,
        OBJECT,
        OL,
        OPTGROUP,
        OPTION,
        OUTPUT,
        P,
        PARAM,
        PICTURE,
        PRE,
        PROGRESS,
        Q,
        RP,
        RT,
        RUBY,
        S,
        SAMP,
        SCRIPT,
        SECTION,
        SELECT,
        SMALL,
        SOURCE,
        SPAN,
        STRONG,
        STYLE,
        SUB,
        SUMMARY,
        SUP,
        SVG,
        TABLE,
        TBODY,
        TD,
        TEMPLATE,
        TEXTAREA,
        TFOOT,
        TH,
        THEAD,
        TIME,
        TITLE,
        TR,
        TRACK,
        U,
        UL,
        VAR,
        VIDEO,
        WBR
    }

    public class HtmlNode
    {
        public HtmlNode(TagType type, string? text = default)
        {
            SelfClosing = TagIsSelfClosing(type);
            Text = text;
            Children = new();
            IsOpen = true;
        }

        public bool IsOpen;

        /// <summary>
        /// The type of element
        /// </summary>
        public TagType Type { get; set; }

        /// <summary>
        /// The text associated with the element
        /// </summary>
        public string? Text { get; set; } = string.Empty;

        /// <summary>
        /// Parent node
        /// </summary>
        public HtmlNode? Parent = null;

        /// <summary>
        /// Child nodes
        /// </summary>
        public List<HtmlNode> Children { get; set; }

        /// <summary>
        /// Describes whether this tag is self closing
        /// </summary>
        public bool SelfClosing { get; set; } = false;

        public static bool TagIsSelfClosing(TagType type)
        {
            switch (type)
            {
                case TagType.AREA:
                case TagType.BASE:
                case TagType.BR:
                case TagType.COL:
                case TagType.EMBED:
                case TagType.HR:
                case TagType.IMG:
                case TagType.INPUT:
                case TagType.KEYGEN:
                case TagType.LINK:
                case TagType.META:
                case TagType.PARAM:
                case TagType.SOURCE:
                case TagType.TRACK:
                case TagType.WBR:
                    return true;
                default:
                    return false;
            }
        }

        public static TagType? GetTagType(string node)
        {
            switch (node)
            {
                case "comment":
                    return TagType.COMMENT;
                case "doctype":
                    return TagType.DOCTYPE;
                case "a":
                    return TagType.A;
                case "abbr":
                    return TagType.ABBR;
                case "address":
                    return TagType.ADDRESS;
                case "area":
                    return TagType.AREA;
                case "article":
                    return TagType.ARTICLE;
                case "aside":
                    return TagType.ASIDE;
                case "audio":
                    return TagType.AUDIO;
                case "b":
                    return TagType.B;
                case "base":
                    return TagType.BASE;
                case "bdi":
                    return TagType.BDI;
                case "bdo":
                    return TagType.BDO;
                case "blockquote":
                    return TagType.BLOCKQUOTE;
                case "body":
                    return TagType.BODY;
                case "br":
                    return TagType.BR;
                case "button":
                    return TagType.BUTTON;
                case "canvas":
                    return TagType.CANVAS;
                case "caption":
                    return TagType.CAPTION;
                case "cite":
                    return TagType.CITE;
                case "code":
                    return TagType.CODE;
                case "col":
                    return TagType.COL;
                case "colgroup":
                    return TagType.COLGROUP;
                case "data":
                    return TagType.DATA;
                case "datalist":
                    return TagType.DATALIST;
                case "dd":
                    return TagType.DD;
                case "del":
                    return TagType.DEL;
                case "details":
                    return TagType.DETAILS;
                case "dfn":
                    return TagType.DFN;
                case "dialog":
                    return TagType.DIALOG;
                case "div":
                    return TagType.DIV;
                case "dl":
                    return TagType.DL;
                case "dt":
                    return TagType.DT;
                case "em":
                    return TagType.EM;
                case "embed":
                    return TagType.EMBED;
                case "fieldset":
                    return TagType.FIELDSET;
                case "figcaption":
                    return TagType.FIGCAPTION;
                case "figure":
                    return TagType.FIGURE;
                case "footer":
                    return TagType.FOOTER;
                case "form":
                    return TagType.FORM;
                case "h1":
                    return TagType.H1;
                case "h2":
                    return TagType.H2;
                case "h3":
                    return TagType.H3;
                case "h4":
                    return TagType.H4;
                case "h5":
                    return TagType.H5;
                case "h6":
                    return TagType.H6;
                case "head":
                    return TagType.HEAD;
                case "header":
                    return TagType.HEADER;
                case "hr":
                    return TagType.HR;
                case "html":
                    return TagType.HTML;
                case "i":
                    return TagType.I;
                case "iframe":
                    return TagType.IFRAME;
                case "img":
                    return TagType.IMG;
                case "input":
                    return TagType.INPUT;
                case "ins":
                    return TagType.INS;
                case "kbd":
                    return TagType.KBD;
                case "keygen":
                    return TagType.KEYGEN;
                case "label":
                    return TagType.LABEL;
                case "legend":
                    return TagType.LEGEND;
                case "li":
                    return TagType.LI;
                case "link":
                    return TagType.LINK;
                case "main":
                    return TagType.MAIN;
                case "map":
                    return TagType.MAP;
                case "mark":
                    return TagType.MARK;
                case "meta":
                    return TagType.META;
                case "meter":
                    return TagType.METER;
                case "nav":
                    return TagType.NAV;
                case "noscript":                
                    return TagType.NOSCRIPT;
                case "object":
                    return TagType.OBJECT;
                case "ol":
                    return TagType.OL;
                case "optgroup":
                    return TagType.OPTGROUP;
                case "option":
                    return TagType.OPTION;
                case "output":
                    return TagType.OUTPUT;
                case "p":
                    return TagType.P;
                case "param":
                    return TagType.PARAM;
                case "picture":
                    return TagType.PICTURE;
                case "pre":
                    return TagType.PRE;
                case "progress":
                    return TagType.PROGRESS;
                case "q":
                    return TagType.Q;
                case "rp":
                    return TagType.RP;
                case "rt":
                    return TagType.RT;
                case "ruby":
                    return TagType.RUBY;
                case "s":
                    return TagType.S;
                case "samp":
                    return TagType.SAMP;
                case "script":
                    return TagType.SCRIPT;
                case "section":
                    return TagType.SECTION;
                case "select":
                    return TagType.SELECT;
                case "small":
                    return TagType.SMALL;
                case "source":
                    return TagType.SOURCE;
                case "span":
                    return TagType.SPAN;
                case "strong":
                    return TagType.STRONG;
                case "style":
                    return TagType.STYLE;
                case "sub":
                    return TagType.SUB;
                case "summary":
                    return TagType.SUMMARY;
                case "sup":
                    return TagType.SUP;
                case "svg":
                    return TagType.SVG;
                case "table":
                    return TagType.TABLE;
                case "tbody":
                    return TagType.TBODY;
                case "td":
                    return TagType.TD;
                case "template":
                    return TagType.TEMPLATE;
                case "textarea":
                    return TagType.TEXTAREA;
                case "tfoot":
                    return TagType.TFOOT;
                case "th":
                    return TagType.TH;
                case "thead":
                    return TagType.THEAD;
                case "time":
                    return TagType.TIME;
                case "title":
                    return TagType.TITLE;
                case "tr":
                    return TagType.TR;
                case "track":
                    return TagType.TRACK;
                case "u":
                    return TagType.U;
                case "ul":
                    return TagType.UL;
                case "var":
                    return TagType.VAR;
                case "video":
                    return TagType.VIDEO;
                case "wbr":
                    return TagType.WBR;
                default:
                    return null;
            }
        }
    }
}
