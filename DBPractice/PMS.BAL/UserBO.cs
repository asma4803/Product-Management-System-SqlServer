using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public class UserBO
    {
        public static int Save(UserDTO dto)
        {
            return PMS.DAL.UserDAO.Save(dto);
        }

        public static int UpdatePassword(UserDTO dto)
        {
            return PMS.DAL.UserDAO.UpdatePassword(dto);
        }

        public static int findUserByEmail(string email)
        {
            return PMS.DAL.UserDAO.findUserByEmail(email);
        }

        public static UserDTO ValidateUser(String pLogin, String pPassword)
        {
            return PMS.DAL.UserDAO.ValidateUser(pLogin, pPassword);
        }
        public static UserDTO GetUserById(int pid)
        {
            return PMS.DAL.UserDAO.GetUserById(pid);
        }
        public static List<UserDTO> GetAllUsers()
        {
            return PMS.DAL.UserDAO.GetAllUsers();
        }

        public static int DeleteUser(int pid)
        {
            return PMS.DAL.UserDAO.DeleteUser(pid);
        }
        public static bool SendEmail(string toEmailAddress, string subject, string body)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                MailAddress to = new MailAddress(toEmailAddress);
                mail.To.Add(to);
                MailAddress from = new MailAddress("asma.eadf15@gmail.com", "UMS");
                mail.From = from;
                mail.Subject = subject;
                mail.Body = body;
                var sc = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("asma.eadf15@gmail.com", "a1s1m1a1"),
                    EnableSsl = true
                };
                sc.Send(mail);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
