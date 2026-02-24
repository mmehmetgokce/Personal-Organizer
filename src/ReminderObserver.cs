using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace PersonalOrganizer
{
    public interface IReminderObserver
    {
        void Update(string reminderSummary);
    }

    public class ReminderSubject
    {
        private IReminderObserver observer;
        private Timer shakeTimer;
        private int shakeCount = 0;
        private Point originalLocation;
        private Form targetForm;

        public ReminderSubject(Form form)
        {
            targetForm = form;
            shakeTimer = new Timer();
            shakeTimer.Interval = 50;
            shakeTimer.Tick += ShakeTimer_Tick;
        }

        public void Attach(IReminderObserver observer)
        {
            this.observer = observer;
        }

        public void Notify(string reminderSummary)
        {
            observer?.Update(reminderSummary);
            StartShakeAnimation();
        }

        private void StartShakeAnimation()
        {
            originalLocation = targetForm.Location;
            shakeCount = 0;
            shakeTimer.Start();
        }

        private void ShakeTimer_Tick(object sender, EventArgs e)
        {
            if (shakeCount >= 40) // 2 saniye (40 * 50ms)
            {
                shakeTimer.Stop();
                targetForm.Location = originalLocation;
                return;
            }

            int offset = (shakeCount % 2 == 0) ? 5 : -5;
            targetForm.Location = new Point(
                originalLocation.X + offset,
                originalLocation.Y
            );
            shakeCount++;
        }
    }

    public class ReminderObserver : IReminderObserver
    {
        private Form targetForm;

        public ReminderObserver(Form form)
        {
            targetForm = form;
        }

        public void Update(string reminderSummary)
        {
            if (targetForm.InvokeRequired)
            {
                targetForm.Invoke(new Action(() => Update(reminderSummary)));
                return;
            }

            targetForm.Text = $"Hatırlatıcı: {reminderSummary}";
        }
    }
} 