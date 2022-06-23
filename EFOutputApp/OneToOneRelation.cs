using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFOutputApp
{
    public class OneToOneRelation
    {
        private readonly DbContextOptions<EFtestdbContext> _options;
        public OneToOneRelation(DbContextOptions<EFtestdbContext> options)
        {
            _options = options;
        }

        public void AddData()
        {
            using (EFtestdbContext db = new EFtestdbContext(_options))
            {
                User user1 = new User { Nickname = "test one-to-one", Phone = "89349083344", IsConfirmed = false, Email = "testtest@mail.ru", Password = "testtest" };
                User user2 = new User { Nickname = "test one-to-one2", Phone = "89123330567", IsConfirmed = false, Email = "test23r@mail.ru", Password = "testtest" };
                db.Users.AddRange(user1, user2);

                UserProfile profile1 = new UserProfile { Name="Sasha", Status="", Reason="", Surname = "Kosarev", User = user1 };
                UserProfile profile2 = new UserProfile { Name = "Masha", Status = "", Reason = "", Surname = "Smirnova", User = user2 };
                db.UserProfiles.AddRange(profile1, profile2);

                db.SaveChanges();
            }
        }

        public void GetData()
        {
            using (EFtestdbContext db = new EFtestdbContext(_options))
            {
                foreach (User user in db.Users.Include(u => u.Profile).ToList())
                {
                    Console.WriteLine($"Name: {user.Profile?.Name} Surname: {user.Profile?.Surname}");
                    Console.WriteLine($"Nickname: {user.Nickname} Password: {user.Password}");
                }
            }
        }

        //public void EditData()
        //{
        //    using (EFtestdbContext db = new EFtestdbContext(_options))
        //    {
        //        User? user = db.Users.FirstOrDefault();
        //        if(user != null)
        //        {
        //            user.Password = "newpassword2022";
        //            db.SaveChanges();
        //        }

        //        UserProfile? userProfile = db.UserProfiles.FirstOrDefault(p => p.User.Nickname == "test one-to-one");
        //        if(userProfile != null)
        //        {
        //            userProfile.Name = "Danill";
        //            db.SaveChanges();
        //        }
        //    }
        //}

        //public void DeleteData()
        //{
        //    using (EFtestdbContext db = new EFtestdbContext(_options))
        //    {
        //        User? user = db.Users.FirstOrDefault();
        //        if(user != null)
        //        {
        //            // в данном случае, т.к. UserProfile требует наличия User, то при удалении User, объект UserProfile тоже будет удален
        //            db.Users.Remove(user);
        //            db.SaveChanges();
        //        }

        //        UserProfile? profile = db.UserProfiles.FirstOrDefault(p => p.User.Nickname == "test one-to-one2");
        //        if(profile != null)
        //        {
        //            db.UserProfiles.Remove(profile);
        //            db.SaveChanges();
        //        }
        //    }
        //}
    }
}
