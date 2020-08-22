﻿using ITAcademy.TaskTwo.Data.Enums;
using ITAcademy.TaskTwo.Data.Models;
using System.Collections.Generic;

namespace ITAcademy.TaskTwo.Web.ViewModels.PhoneVM
{
    public class PhoneIndex
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string SurName { get; set; }

        public int? PrimaryPhoneId { get; set; }

        public MessageType Communication { get; set; }

        public List<Phone> Phones { get; set; }
    }
}