﻿namespace MRSES.Core.Entities
{
    public interface IDatabase
    {
        System.Threading.Tasks.Task SaveAsync();
        System.Threading.Tasks.Task DeleteAsync();
    }
}
