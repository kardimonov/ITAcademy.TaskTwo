﻿using ITAcademy.TaskTwo.Data.Interfaces;
using ITAcademy.TaskTwo.Data.Models;

namespace ITAcademy.TaskTwo.Logic.Interfaces
{
    public interface IPhoneDecorator : IRepository<Phone>, IBaseDecorator
    {
    }
}