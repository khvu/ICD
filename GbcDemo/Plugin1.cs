using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using SharpAccessory.Resources;

using SharpAccessory.GenericBusinessClient.Plugging;
using SharpAccessory.GenericBusinessClient.VisualComponents;

using SharpAccessory.GenericBusinessClient.DefaultPlugins;


namespace GbcDemo
{
	
	public class Plugin1:Plugin
	{
		private ToolButton tbGoToSearchForm;

        private Label lbl = new Label();

		
		public Plugin1()
		{
			Text="Display Selected Records";
			
			tbGoToSearchForm=new ToolButton();
			tbGoToSearchForm.Image=TangoIconSet.LoadIcon(TangoIcon.Go_Next);
			tbGoToSearchForm.Text="Search form";
			tbGoToSearchForm.Click+=delegate{ShowPlugin("Plugin2");};
			
            //tbShowMessage=new ToolButton();
            //tbShowMessage.Image=TangoIconSet.LoadIcon(TangoIcon.Window_New);
            //tbShowMessage.Text="Show Message";
            //tbShowMessage.Click+=delegate{ShowMessage("Test!");};

            lbl.Text = "";
		}
		
		
		protected override HomepageInfo GetHomepageInfo()
		{
			HomepageInfo info=HomepageInfo.Empty;
			
			info.Image=TangoIconSet.LoadIcon(TangoIcon.Go_Home);
			info.Name="Plugin 1";
			
			return info;
		}
		
		
		protected override ToolButton[] GetToolButtons()
		{
			return new ToolButton[]{ tbGoToSearchForm };
		}

        protected override void OnShow(ShowEventArgs e)
        {
            if (e.Argument != null)
            {
                    lbl.Location = new Point(0, 0);
                    lbl.MaximumSize = new Size(1000, 0);
                    lbl.AutoSize = true;
                    lbl.Text = e.Argument.ToString();
                    lbl.Parent = Control;
            }
        }
	}
}