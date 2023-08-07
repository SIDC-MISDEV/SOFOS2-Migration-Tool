using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOFOS2_Migration_Tool.Helper
{
    public class ThreadHelper
    {
        delegate void SetLabelText(Form f, Control c, string text);

        public static void SetLabel(Form f, Control c, string val)
        {
            if(c.InvokeRequired)
            {
                SetLabelText d = new SetLabelText(SetLabel);
                f.Invoke(d, new object[] { f, c, val });
            }
            else
            {
                c.Text = val;

                if (val.Contains("Exception"))
                    c.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
