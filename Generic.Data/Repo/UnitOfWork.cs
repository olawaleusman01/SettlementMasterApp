﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Generic.Data
{
    
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMyContext context;
        public UnitOfWork()
        {
            context = new MyContext();
        }
        public UnitOfWork(MyContext context)
        {
            this.context = context;
        }
        public int Save(string userid,string authId = null)
        {
            return context.SaveChanges(userid,authId);
        }
        public IMyContext Context
        {
            get
            {
                return context;
            }
        }
        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }
}
