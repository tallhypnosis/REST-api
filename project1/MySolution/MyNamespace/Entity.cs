using System;
using System.Collections.Generic;
using MyNamespace.Controllers;

namespace MyNamespace.Controllers{
    public class Entity{
        public string? Id {get;set;}
        public List<Address>? Addresses {get;set;}
        public List<Date> Dates {get;set;} = new List<Date>();
        public bool Deceased {get; set;}
        public string? Gender {get; set;}
        public List<Name>? Names {get; set;}
    }

    public class Address{
        public string? AddressLine {get; set;}
        public string? City {get; set;}
        public string? Country{get; set;}
    }

    public class Date{
        public string? DateType {get; set;}
        public DateTime? DateValue {get; set;}
    }

    public class Name{
        public string? FirstName {get; set;}
        public string? MiddleName {get; set;}
        public string? LastName {get; set;}
    }
}