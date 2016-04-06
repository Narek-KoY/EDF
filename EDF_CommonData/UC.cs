using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace EDF_CommonData
{
    public class UC
    {
        public static void SearchBox(WebControl tb, string defaulText)
        {
            tb.Attributes.Add("value", defaulText);
            tb.Attributes.Add("onFocus", @"if(this.value == '" + defaulText + "') {this.value = '';}");
            tb.Attributes.Add("onBlur", @"if (this.value == '') {this.value = '" + defaulText + "';}");
        }

        public static void SearchBox(HtmlTextArea tb, string defaulText)
        {
            tb.Attributes.Add("value", defaulText);
            tb.Attributes.Add("onFocus", @"if(this.value == '" + defaulText + "') {this.value = '';}");
            tb.Attributes.Add("onBlur", @"if (this.value == '') {this.value = '" + defaulText + "';}");
        }

        public static bool RequiredVal(HtmlTextArea textArea)
        {
            return textArea.InnerText != string.Empty;
        }

        public static bool RequiredVal(HtmlTextArea TextArea, ref HtmlGenericControl Label, string ErrorMessage)
        {
            if (!TextArea.Visible)
                return true;

            if (TextArea.InnerText == string.Empty || TextArea.InnerText.Contains("Write") || TextArea.InnerText.Contains("Select"))
            {
                Label.InnerText = ErrorMessage;
                return false;
            };

            Label.InnerText = string.Empty;
            return true;
        }

        public static bool RequiredVal(HtmlTextArea textArea, ref Label label, string errorMessage)
        {
            if (!textArea.Visible)
                return true;

            if (textArea.InnerText == string.Empty || textArea.InnerText.Contains("Write") || textArea.InnerText.Contains("Select"))
            {
                label.Text = errorMessage;
                return false;
            };

            label.Text = string.Empty;
            return true;
        }

        public static bool RequiredVal(TextBox textBox, ref HtmlGenericControl label, string errorMessage)
        {
            if (!textBox.Visible)
                return true;

            if (textBox.Text == string.Empty || textBox.Text.Contains("Write") || textBox.Text.Contains("Select"))
            {
                label.InnerText = errorMessage;
                return false;
            };

            label.InnerText = string.Empty;
            return true;
        }

        public static bool RequiredVal(TextBox textBox, ref Label label, string errorMessage)
        {
            if (!textBox.Visible)
                return true;

            if (textBox.Text == string.Empty || textBox.Text.Contains("Write") || textBox.Text.Contains("Select"))
            {
                label.Text = errorMessage;
                return false;
            };

            label.Text = string.Empty;
            return true;
        }

        public bool TryHide(ref HtmlGenericControl label)
        {
            return !string.IsNullOrEmpty(label.InnerText);
        }
    }
}