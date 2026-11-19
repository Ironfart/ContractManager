using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractManager.Data {
  public class User {
    public int id { get; set; }

    public string name { get; set; }

    public string username { get; set; }

    public string userType { get; set; }

    public int userTypeId { get; set; }
  }
}