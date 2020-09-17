using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Group_Project
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Group_Service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Group_Service.svc or Group_Service.svc.cs at the Solution Explorer and start debugging.
    public class Group_Service : IGroup_Service
    {

        // connecting to the database
        DataClasses1DataContext db = new DataClasses1DataContext();

        //User Management
        //getting admin
        public Admin getAdmin(int id)
        {
            var ad = (from a in db.Admins
                      where a.User_Id.Equals(id)
                      select a).FirstOrDefault();

            if (ad == null)
            {
                return null;
            }
            else
            {
                return ad;
            }
        }


        //getting mask
        public Mask_Type getMask(int id)
        {
            var ms = (from c in db.Mask_Types
                      where c.Mask_Id.Equals(id)
                      select c).FirstOrDefault();

            if (ms == null)
            {
                return null;
            }
            else
            {
                return ms;
            }
        }

        //getting client
        public Client getClient(int id)
        {
            var cl = (from c in db.Clients
                      where c.User_Id.Equals(id)
                      select c).FirstOrDefault();

            if (cl == null)
            {
                return null;
            }
            else
            {
                return cl;
            }
        }


        // function to get email from the tabe
        public User_Table getEmail(string email, int id)
        {

            var us = (from e in db.User_Tables
                      where e.Email.Equals(email) && e.User_Id != id
                      select e).FirstOrDefault();

            if (us == null)
            {
                return null;
            }
            else
            {
                return us;
            }

        }

        public string Changepassword(string email, int id, string password)
        {
            var eemail = getEmail(email, id);

            if (eemail == null)
            {
                var user = getUser(id);

                if (user != null)
                {
                    user.Password = password;
                   
                    try
                    {
                        //update
                        db.SubmitChanges();
                        return " updated";
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        ex.GetBaseException();
                        return "unsuccessful update";
                    }
                }
                else
                {
                    // needs to register user
                    return "unregistred";
                }
            }
            else
            {
                return "unsuccessful update";
            }
        }

        //getiing user
        public User_Table getUser(int id)
        {

            var us = (from u in db.User_Tables
                      where u.User_Id.Equals(id)
                      select u).FirstOrDefault();

            if (us == null)
            {
                return null;
            }
            else
            {
                return us;
            }

        }

        //function to login
        public int login(string email, string password)
        {
            //check if the user's information is in the database
            var us = (from u in db.User_Tables
                      where u.Email.Equals(email) && u.Password.Equals(password)
                      select u).FirstOrDefault();

            if (us != null)
            {
                return us.User_Id;
            }
            else
            {
                return 0;
            }
        }



        //register according to user type
        public string Register(string username, string password, string name, string email, string contactno, int active, string address, string surname = null, string businesstype = null, string usertype = "client")
        {
            var user = (from u in db.User_Tables
                        where u.Email.Equals(email)
                        select u).FirstOrDefault();

            if (user == null)
            {
                var newUser = new User_Table
                {
                    Username = username,
                    //Surname = surname,
                    Password = password,
                    Name = name,
                    Email = email,
                    Contact_Number = contactno,
                    Date_Created = DateTime.Today,
                    Active = active,
                    Address = address,
                    Usertype = usertype


                };

                if (usertype == "admin")
                {
                    Admin a = new Admin
                    {
                        Surname = surname
                    };
                }
                else if (usertype == "client")
                {
                    Client c = new Client
                    {
                        Business_Type = businesstype
                    };
                }
                else
                {
                    return "unsuccessful";
                }
                db.User_Tables.InsertOnSubmit(newUser);

                try
                {
                    db.SubmitChanges();
                    return "registered";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful";
                }
            }
            else
            {
                return "unsuccessful";
            }
        }

        //updating according to the type of user
        public string UpdateInfo(string username, string name, string email, string contactno, string address, int id, string surname = null, string businesstype = null)
        {

            var eemail = getEmail(email, id);

            if (eemail == null)
            {
                var user = getUser(id);

                if (user != null)
                {
                    user.Username = username;
                    user.Name = name;
                    user.Email = email;
                    user.Contact_Number = contactno;
                    user.Address = address;

                    if (user.Usertype == "admin")
                    {
                        //getting admin and changing the surname
                        var a = getAdmin(id);
                        a.Surname = surname;

                    }
                    else if (user.Usertype == "client")
                    {
                        //if client change the business type if they wish to change it
                        var c = getClient(id);
                        c.Business_Type = businesstype;
                    }
                    try
                    {
                        //update
                        db.SubmitChanges();
                        return " updated";
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        ex.GetBaseException();
                        return "unsuccessful update";
                    }
                }
                else
                {
                    // needs to register user
                    return "unregistred";
                }
            }
            else
            {
                return "unsuccessful update";
            }
        }

        //
        public string AddGuest(int id, string names, string address, int orderid)
        {
            var guest = (from g in db.Guests
                         where g.Guest_Id.Equals(id)
                         select g).FirstOrDefault();

            if (guest == null)
            {
                var newguest = new Guest
                {
                    Guest_FullName = names,
                    Guest_Address = address,
                    Order_Id = orderid

                };

                db.Guests.InsertOnSubmit(newguest);

                try
                {
                    db.SubmitChanges();
                    return "guest added";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful";
                }
            }
            else
            {
                return "unsuccessful";
            }
        }

        public string deleteUser(int id)
        {
            var user = getUser(id);

            var del =       from u in db.User_Tables
                            where u.User_Id.Equals(id)
                            select u;

            foreach (var us in del)
            {
                db.User_Tables.DeleteOnSubmit(us);
            }

            try
            {
                db.SubmitChanges();
                return " Deleted";
            }
            catch (Exception e)
            {
                return "User doesn't exist";
            }

        }














        //getting invoice
        public Order_Table getInvoice(int id)
        {
            var order = (from o in db.Order_Tables
                         where o.Order_Id.Equals(id)
                         select o).FirstOrDefault();

            if (order == null)
            {
                return null;
            }
            else
            {
                return order;
            }
        }

        //getting item
        public Order_Item getItem(int id)
        {
            var item = (from i in db.Order_Items
                        where i.Order_Id.Equals(id)
                        select i).FirstOrDefault();

            if (item == null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }



        public List<Product> getProductsbycategory(int maskid)
        {
            var p = new List<Product>();

            dynamic prod = (from t in db.Products
                            where t.Mask_Id.Equals(maskid)
                            select t);

            foreach (Product pro in prod)
            {
                var ps = getProduct(pro.Product_Id);
                p.Add(ps);
            }

            return p;
        }

        public List<Order_Item> getallitems()
        {
            var o = new List<Order_Item>();

            dynamic prod = (from t in db.Order_Items
                            select t);

            foreach (Order_Table or in prod)
            {
                var ord = getItem(or.Order_Id);
                o.Add(ord);
            }

            return o;
        }

        public List<Order_Table> getallInvoices()
        {
            var o = new List<Order_Table>();

            dynamic prod = (from t in db.Order_Tables
                            select t);

            foreach (Order_Table or in prod)
            {
                var ord = getInvoice(or.Order_Id);
                o.Add(ord);
            }

            return o;
        }

        public List<Order_Table> getInvoicebyclient(int clientid)
        {
            var o = new List<Order_Table>();

            dynamic prod = (from t in db.Order_Tables
                            where t.Client.User_Id.Equals(clientid)
                            select t);

            foreach (Order_Table or in prod)
            {
                var ord = getInvoice(or.Order_Id);
                o.Add(ord);
            }

            return o;
        }

        public List<Order_Table> getInvoicebbydate(DateTime d)
        {
            var o = new List<Order_Table>();

            dynamic prod = (from t in db.Order_Tables
                            where t.Order_date.Equals(d)
                            select t);

            foreach (Order_Table or in prod)
            {
                var ord = getInvoice(or.Order_Id);
                o.Add(ord);
            }

            return o;
        }

       







        //add products
        public string addproducts(string name, string description, Decimal price, int active, int maskid, int admin, int quantity)
        {
            var prod = (from p in db.Products
                        where p.Name.Equals(name)
                        select p).FirstOrDefault();

            var a = getAdmin(admin);
            a.User_Id = admin;

            var m = getMask(maskid);
            m.Mask_Id = maskid;

            if (prod == null)
            {
                var newprod = new Product
                {
                    Name = name,
                    Description = description,
                    Unit_Price = price,
                    Active = active,
                    Date_Created = DateTime.Today,
                    Product_Quantity = quantity
                };
                db.Products.InsertOnSubmit(newprod);

                try
                {
                    db.SubmitChanges();
                    return "added";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "not added";
                }
            }
            else
            {
                return "error";
            }


        }

        public string editproduct(string name, string description, Decimal price, int id, int active, int maskid, int admin, int quantity)
        {
            var prod = getProduct(id);

            if (prod != null)
            {
                prod.Name = name;
                prod.Description = description;
                prod.Unit_Price = price;
                prod.Product_Quantity = quantity;
                var a = getAdmin(admin);
                a.User_Id = admin;

                var m = getMask(maskid);
                m.Mask_Id = maskid;
                try
                {
                    //update
                    db.SubmitChanges();
                    return " updated";
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful update";
                }
            }
            else
            {
                // needs to register user
                return "product does not exist";
            }
        }

        public string addtype(string name, string description, int admin)
        {
            var ty = (from p in db.Mask_Types
                      where p.Name.Equals(name)
                      select p).FirstOrDefault();

            var a = getAdmin(admin);
            a.User_Id = admin;

            if (ty == null)
            {
                var newtype = new Mask_Type
                {
                    Name = name,
                    Description = description,

                };
                db.Mask_Types.InsertOnSubmit(newtype);

                try
                {
                    db.SubmitChanges();
                    return "added";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "not added";
                }
            }
            else
            {
                return "error";
            }
        }

        public string edittype(string name, string description, int admin, int id)
        {
            var ty = getMask(id);

            if (ty != null)
            {
                ty.Name = name;
                ty.Description = description;
                var a = getAdmin(admin);
                a.User_Id = admin;
                try
                {
                    //update
                    db.SubmitChanges();
                    return " updated";
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful update";
                }
            }
            else
            {

                return "type does not exist";
            }
        }

        public string addsize(string name, string dimensions)
        {
            var ty = (from p in db.Mask_Types
                      where p.Name.Equals(name)
                      select p).FirstOrDefault();


            if (ty == null)
            {
                var newtype = new Size_Table
                {
                    Name = name,
                    Dimensions = dimensions,

                };
                db.Size_Tables.InsertOnSubmit(newtype);

                try
                {
                    db.SubmitChanges();
                    return "added";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "not added";
                }
            }
            else
            {
                return "error";
            }
        }

        public string addpsize(int sizeid, int psize)
        {
            var ty = (from p in db.Product_Sizes
                      where p.Size_Id.Equals(sizeid) && p.Product_Id.Equals(psize)
                      select p).FirstOrDefault();

            if (ty == null)
            {
                var newtype = new Product_Size
                {
                    Size_Id = sizeid,
                    Product_Id = psize

                };
                db.Product_Sizes.InsertOnSubmit(newtype);

                try
                {
                    db.SubmitChanges();
                    return "added";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "not added";
                }
            }
            else
            {
                return "error";
            }
        }

        public string addcustom(int pid, int filter, string size)
        {
            var ty = (from p in db.Custom_Products
                      where p.Filter.Equals(filter)
                      select p).FirstOrDefault();


            if (ty == null)
            {
                var newtype = new Custom_Product
                {
                    Filter = filter,
                    Custom_Size = size

                };
                db.Custom_Products.InsertOnSubmit(newtype);

                try
                {
                    db.SubmitChanges();
                    return "added";
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return "not added";
                }
            }
            else
            {
                return "error";
            }

        }

        public string editsize(string name, string dimen, int id)
        {
            var ty = getsize(id);

            if (ty != null)
            {
                ty.Name = name;
                ty.Dimensions = dimen;
            
                try
                {
                    //update
                    db.SubmitChanges();
                    return " updated";
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful update";
                }
            }
            else
            {

                return "size does not exist";
            }
        }

        public string updatepsize(int sizeid, int psize, int id)
        {
            var ty = getproductsize(psize,sizeid);

            if (ty != null)
            {
                ty.Size_Id = sizeid;
                ty.Product_Id = psize;
                try
                {
                    //update
                    db.SubmitChanges();
                    return " updated";
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful update";
                }
            }
            else
            {

                return "product size does not exist";
            }
        }

        public string editcustom(int pid, int filter, string size, int id)
        {
            var ty = getcustom(id);

            if (ty != null)
            {
                ty.Filter = filter;
                ty.Custom_Size = size;
                try
                {
                    //update
                    db.SubmitChanges();
                    return " updated";
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful update";
                }
            }
            else
            {

                return "custom product does not exist";
            }
        }

        public List<Product> getallproducts()
        {
            var prods = new List<Product>();

            dynamic prod = (from t in db.Products
                            select t);

            foreach (Product p in prod)
            {
                var ps = getProduct(p.Product_Id);
                prods.Add(ps);
            }

            return prods;
        }

        public List<Mask_Type> getallMasktypes()
        {
            var type = new List<Mask_Type>();

            dynamic prod = (from t in db.Mask_Types
                            select t);

            foreach (Product p in prod)
            {
                var ps = getMask(p.Mask_Id);
                type.Add(ps);
            }

            return type;
        }

        public Product getProduct(int id)
        {
            var us = (from p in db.Products
                      where p.Product_Id.Equals(id)
                      select p).FirstOrDefault();

            if (us == null)
            {
                return null;
            }
            else
            {
                return us;
            }
        }

        public Custom_Product getcustom(int id)
        {

            var us = (from p in db.Custom_Products
                      where p.Product_Id.Equals(id)
                      select p).FirstOrDefault();

            if (us == null)
            {
                return null;
            }
            else
            {
                return us;
            }
        }

        public Size_Table getsize(int id)
        {

            var us = (from p in db.Size_Tables
                      where p.Size_Id.Equals(id)
                      select p).FirstOrDefault();

            if (us == null)
            {
                return null;
            }
            else
            {
                return us;
            }
        }

        


        public Product_Size getproductsize(int pid, int sid)
        {

            var us = (from p in db.Product_Sizes
                      where p.Product_Id.Equals(pid) && p.Size_Id.Equals(sid)
                      select p).FirstOrDefault();

            if (us == null)
            {
                return null;
            }
            else
            {
                return us;
            }
        }

        public string deletesize(int id)
        {

            var del = from s in db.Size_Tables
                      where s.Size_Id.Equals(id)
                      select s;

            foreach (var siz in del)
            {
                db.Size_Tables.DeleteOnSubmit(siz);
            }

            try
            {
                db.SubmitChanges();
                return " Deleted";
            }
            catch (Exception e)
            {
                return "User doesn't exist";
            }

        }

        public string deletePsize(int pid, int sid)
        {

            var del = from p in db.Product_Sizes
                      where p.Size_Id.Equals(pid) && p.Size_Id.Equals(sid)
                      select p;

            foreach (var us in del)
            {
                db.Product_Sizes.DeleteOnSubmit(us);
            }

            try
            {
                db.SubmitChanges();
                return " Deleted";
            }
            catch (Exception e)
            {
                return "User doesn't exist";
            }

        }

        public string deleteMaskt(int id)
        {

            var del = from m in db.Mask_Types
                      where m.Mask_Id.Equals(id)
                      select m;

            foreach (var us in del)
            {
                db.Mask_Types.DeleteOnSubmit(us);
            }

            try
            {
                db.SubmitChanges();
                return " Deleted";
            }
            catch (Exception e)
            {
                return "User doesn't exist";
            }

        }

        public string deletecustom(int id)
        {

            var del = from p in db.Custom_Products
                      where p.Product_Id.Equals(id)
                      select p;

            foreach (var us in del)
            {
                db.Custom_Products.DeleteOnSubmit(us);
            }

            try
            {
                db.SubmitChanges();
                return " Deleted";
            }
            catch (Exception e)
            {
                return "User doesn't exist";
            }

        }

        public string deleteProduct(int P_Id)
        {
            //Get product based off ID
            var delete = (from p in db.Products
                          where p.Product_Id.Equals(P_Id)
                          select p).FirstOrDefault();
            if (delete != null)
            {
                //Set the active attribute to 0
                delete.Active = 0;

                try
                {
                    //Submit changes 
                    db.SubmitChanges();
                    return "Deleted";
                }
                catch (Exception e)
                {
                    return "Product not deleted";
                }
            }
            else
            {
                return "Product doesn't exist";
            }
        }


    
    /*   public List<Delivery> GetAllDeliveries()
        {

        }

         public List<Delivery> GetDeliveriesForClient()
        {

        }

       public List<Delivery> GetDeliveriesByCompany()
        {

        }
        //<-----Adding To Order----->
  
        public int AddOrderItem()
        {

        }
    */
    }
}

