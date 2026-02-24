using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PersonalOrganizer
{
    public static class EmailService
    {
        private static readonly string SmtpHost = "smtp.example.com";
        private static readonly int SmtpPort = 587;
        private static readonly string SmtpUsername = "personalorganizer@example.com";
        private static readonly string SmtpPassword = "yourpassword"; // Gerçek bir uygulamada güvenli bir şekilde saklanmalıdır
        private static readonly string FromEmail = "personalorganizer@example.com";
        private static readonly string FromName = "Kişisel Organizatör";

        /// <summary>
        /// Şifre sıfırlama e-postası gönderir
        /// </summary>
        /// <param name="toEmail">Alıcı e-posta adresi</param>
        /// <param name="resetCode">Şifre sıfırlama kodu</param>
        /// <returns>E-posta gönderildi mi?</returns>
        public static bool SendPasswordResetEmail(string toEmail, string resetCode)
        {
            string subject = "Kişisel Organizatör - Şifre Sıfırlama";
            
            StringBuilder body = new StringBuilder();
            body.AppendLine("<html><body>");
            body.AppendLine("<h2>Kişisel Organizatör - Şifre Sıfırlama</h2>");
            body.AppendLine("<p>Merhaba,</p>");
            body.AppendLine("<p>Şifrenizi sıfırlamak için aşağıdaki kodu kullanabilirsiniz:</p>");
            body.AppendLine($"<h3>{resetCode}</h3>");
            body.AppendLine("<p>Bu kod 30 dakika süreyle geçerlidir.</p>");
            body.AppendLine("<p>Eğer şifre sıfırlama talebinde bulunmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>");
            body.AppendLine("<p>Saygılarımızla,<br>Kişisel Organizatör Ekibi</p>");
            body.AppendLine("</body></html>");

            return SendEmail(toEmail, subject, body.ToString(), true);
        }

        /// <summary>
        /// Genel e-posta gönderme metodu
        /// </summary>
        /// <param name="toEmail">Alıcı e-posta adresi</param>
        /// <param name="subject">E-posta konusu</param>
        /// <param name="body">E-posta içeriği</param>
        /// <param name="isHtml">İçerik HTML mi?</param>
        /// <returns>E-posta gönderildi mi?</returns>
        public static bool SendEmail(string toEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(SmtpHost, SmtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
                    client.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(FromEmail, FromName);
                        message.To.Add(toEmail);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = isHtml;

                        client.Send(message);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"E-posta gönderilirken hata oluştu: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test amaçlı olarak gerçekte e-posta göndermeden
        /// başarılı olmuş gibi davran (geliştirme aşamasında)
        /// </summary>
        public static bool SendPasswordResetEmailSimulation(string toEmail, string resetCode)
        {
            // Gerçek bir uygulamada, SendPasswordResetEmail kullanılır
            // Geliştirme aşamasında console'a yazdırarak test ediyoruz
            Console.WriteLine($"[SIMÜLASYON] {toEmail} adresine şifre sıfırlama e-postası gönderildi.");
            Console.WriteLine($"[SIMÜLASYON] Şifre sıfırlama kodu: {resetCode}");
            
            // Bilgisayarın bildirim alanında göster (Windows için)
            System.Windows.Forms.MessageBox.Show(
                $"Şifre sıfırlama kodu: {resetCode}\nE-posta: {toEmail}",
                "Şifre Sıfırlama Simülasyonu",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
            
            return true;
        }
    }
} 