﻿using DataAccessLayerT.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerT.Classes
{
   public class LoanViewModel
    {

       
        public ApplicationStatu ApplicationStatu
        {
            get;
            set;
        }

        public StudentRecord StudentRecord
        {
            get;
            set;
        }

        public LoanApplication LoanApplication
        {
            get;
            set;
        }
    }
    
}
