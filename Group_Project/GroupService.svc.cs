using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace Group_Project
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GroupService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GroupService.svc or GroupService.svc.cs at the Solution Explorer and start debugging.
    public class GroupService : IGroupService
    {

        // connecting to the database
        GroupDataClassesDataContext db = new GroupDataClassesDataContext();

        //User Management
        //getting admin
        public Admin GetAdmin(int id)
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
        public Mask_Type GetMask(int id)
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
        public Client GetClient(int id)
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
        public User_Table GetEmail(string email, int id)
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
            var eemail = GetEmail(email, id);

            if (eemail == null)
            {
                var user = GetUser(id);

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
        public User_Table GetUser(int id)
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
        public int Login(string email, string password)
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
                        User_Id = newUser.User_Id,
                        Surname = surname
                    };
                }
                else if (usertype == "client")
                {
                    Client c = new Client
                    {
                        User_Id= newUser.User_Id,
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

            var eemail = GetEmail(email, id);

            if (eemail == null)
            {
                var user = GetUser(id);

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
                        var a = GetAdmin(id);
                        a.Surname = surname;

                    }
                    else if (user.Usertype == "client")
                    {
                        //if client change the business type if they wish to change it
                        var c = GetClient(id);
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

        public string DeleteUser(int id)
        {
            User_Table user = GetUser(id);

            user =( from u in db.User_Tables
                      where u.User_Id.Equals(id)
                      select u).FirstOrDefault();

            if (user == null)
            {
                return "User doesn't exist";
            }
            else
            {
                user.Active = 0;
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


        }

        //<---------------------------------------------Invoices------------------------------------------------>
        //getting invoice
        public Order_Table GetInvoice(int id)
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
        public Order_Item GetItem(int id)
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

        public List<Order_Item> Getallitems()
        {
            var o = new List<Order_Item>();

            dynamic prod = (from t in db.Order_Items
                            select t);

            foreach (Order_Item or in prod)
            {
                var ord = GetItem(or.Order_Id);
                o.Add(ord);
            }

            return o;
        }

        public List<Order_Table> GetallInvoices()
        {
            var o = new List<Order_Table>();

            dynamic prod = (from t in db.Order_Tables
                            select t);

            foreach (Order_Table or in prod)
            {
                var ord = GetInvoice(or.Order_Id);
                o.Add(ord);
            }

            return o;
        }

        public List<Order_Table> GetInvoicebyclient(int clientid)
        {
            var o = new List<Order_Table>();

            dynamic prod = (from t in db.PayClients
                            where t.User_Id.Equals(clientid)
                            select t);
            foreach (PayClient or in prod)
            {
                var ord = GetInvoice(or.Order_Id);
                o.Add(ord);
            }


            return o;
        }

        public List<Order_Table> GetInvoicebbydate(DateTime d)
        {
            var orders = new List<Order_Table>();

            dynamic prod = (from t in db.Order_Tables
                            where t.Order_date.Equals(d)
                            select t);

            if (prod != null)
            {
                foreach (Order_Table o in prod)
                {
                    orders.Add(o);
                }

                return orders;
            }
            else
            {
                return null;
            }
        }

        //add products
        public string Addproducts(string name, string description, Decimal price, int active, int maskid, int admin, int quantity)
        {
            var prod = (from p in db.Products
                        where p.Name.Equals(name)
                        select p).FirstOrDefault();

            var a = GetAdmin(admin);
            a.User_Id = admin;

            var m = GetMask(maskid);
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

        public string Editproduct(string name, string description, Decimal price, int id, int active, int maskid, int admin, int quantity)
        {
            var prod = GetProduct(id);

            if (prod != null)
            {
                prod.Name = name;
                prod.Description = description;
                prod.Unit_Price = price;
                prod.Product_Quantity = quantity;
                var a = GetAdmin(admin);
                a.User_Id = admin;

                var m = GetMask(maskid);
                m.Mask_Id = maskid;
                try
                {
                    //update
                    db.SubmitChanges();
                    return "updated";
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

        public string Addcustom(int filter, string size, string colour, string imageURL = "")
        {
            if (imageURL != null)
            {
                var newtype = new Custom_Product
                {
                    Filter = filter,
                    Custom_Size = size,
                    Colour = colour,
                    Logo = imageURL
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
                var newtype = new Custom_Product
                {
                    Filter = filter,
                    Custom_Size = size,
                    Colour = colour
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
        }

        public string Editcustom(int filter, string size, string colour, int _ID, string imageURL = "")
        {
            //Get custom product based on client id 
            var ty = Getcustom(_ID);

            //If custom product exists
            if (ty != null)
            {
                ty.Filter = filter;
                ty.Custom_Size = size;
                ty.Colour = colour;
                ty.Logo = imageURL;
                try
                {
                    //update
                    db.SubmitChanges();
                    return "updated";
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return "unsuccessful update";
                }
            }
            //Custom product doesn't exists.
            else
            {

                return "custom product does not exist";
            }
        }

        public Custom_Product Getcustom(int _ID)
        {

            var us = (from p in db.Custom_Products
                      where p.Client_ID.Equals(_ID)
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

        public string DeleteProduct(int P_Id)
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
                catch (Exception)
                {
                    return "Product not deleted";
                }
            }
            else
            {
                return "Product doesn't exist";
            }
        }


        public List<Product> GetProductsbycategory(int maskid)
        {
            var p = new List<Product>();

            dynamic prod = (from t in db.Products
                            where t.Mask_Id.Equals(maskid)
                            select t);

            foreach (Product pro in prod)
            {
                var ps = GetProduct(pro.Product_Id);
                p.Add(ps);
            }

            return p;
        }

        public List<Product> Getproductbyprice(decimal min, decimal max)
        {
            var list = new List<Product>();

            dynamic li = (from l in db.Products
                          where l.Unit_Price > min && l.Unit_Price < max
                          select l).ToList();

            foreach (Product p in li)
            {
                var ps = GetProduct(p.Product_Id);
                list.Add(ps);
            }

            return list;
        }

        public List<Product_Size> Getproductbysize(int sizeid)
        {
            var size = new List<Product_Size>();

            dynamic prod = (from t in db.Product_Sizes
                            where t.Size_Id.Equals(sizeid)
                            select t);

            foreach (Product_Size p in prod)
            {
                var ps = Getsproductsize(p.Size_Id);
                size.Add(ps);
            }

            return size;
        }

        public Product_Size Getsproductsize(int sid)
        {
            var prod = (from p in db.Product_Sizes
                        where p.Size_Id.Equals(sid)
                        select p).FirstOrDefault();

            if (prod == null)
            {
                return null;
            }
            else
            {
                return prod;
            }
        }


 
        public string Addtype(string name, string description, int admin)
        {
            var ty = (from p in db.Mask_Types
                      where p.Name.Equals(name)
                      select p).FirstOrDefault();

            var a = GetAdmin(admin);
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

        public string Edittype(string name, string description, int admin, int id)
        {
            var ty = GetMask(id);

            if (ty != null)
            {
                ty.Name = name;
                ty.Description = description;
                var a = GetAdmin(admin);
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

        public string Addsize(string name, string dimensions)
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

        public string Addpsize(int sizeid, int psize)
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

        public string Addcustom(int pid, int filter, string size)
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

        public string Editsize(string name, string dimen, int id)
        {
            var ty = Getsize(id);

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

        public string Updatepsize(int sizeid, int psize, int id)
        {
            var ty = Getproductsize(psize, sizeid);

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

        public string Editcustom(int pid, int filter, string size, int id)
        {
            var ty = Getcustom(id);

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

        public List<Product> Getallproducts()
        {
            var prods = new List<Product>();

            dynamic prod = (from t in db.Products
                            select t);

            foreach (Product p in prod)
            {
                var ps = GetProduct(p.Product_Id);
                prods.Add(ps);
            }

            return prods;
        }

        public List<Mask_Type> GetallMasktypes()
        {
            var type = new List<Mask_Type>();

            dynamic prod = (from t in db.Mask_Types
                            select t);

            foreach (Product p in prod)
            {
                var ps = GetMask(p.Mask_Id);
                type.Add(ps);
            }

            return type;
        }

        public Product GetProduct(int id)
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


        public Size_Table Getsize(int id)
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




        public Product_Size Getproductsize(int pid, int sid)
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

        public string Deletesize(int id)
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

        public string DeletePsize(int pid, int sid)
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

        public string DeleteMaskt(int id)
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

        public string Deletecustom(int id)
        {

            var del = from p in db.Custom_Products
                      where p.CustomProduct_Id.Equals(id)
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






        //<---------------------------------------------Shopping Cart Functions------------------------------------------------->

        //<-----Deliveries----->
        //Returns all the deliveries in the database
        public List<Delivery> GetAllDeliveries()
        {
            List<Delivery> delivery = new List<Delivery>();
            dynamic deliveries = (from d in db.Deliveries
                                  select d).ToList();
            foreach (Delivery d in deliveries)
            {
                delivery.Add(d);
            }

            return delivery;
        }

        //Returns a deliver, given an ID
        public Delivery GetDelivery(int DeliveryID)
        {
            Delivery del = new Delivery();
            del = (from d in db.Deliveries
                   where d.Delivery_Id.Equals(DeliveryID)
                   select d).FirstOrDefault();

            return del;
        }

        //Returns the Delivery for an order
        public Delivery GetDeliveryForOrder(int orderID)
        {
            //Get the order 
            Order_Table ord = GetInvoice(orderID);
            Delivery del = new Delivery();
            del = GetDelivery(ord.Delivery_Id);

            return del;
        }
        //Returns all the deliveries done by a client
        public List<Delivery> GetDeliveriesForClient(int ClientID)
        {
            List<Delivery> delivery = new List<Delivery>();
            dynamic prod = (from t in db.PayClients
                            where t.User_Id.Equals(ClientID)
                            select t);
            foreach (PayClient or in prod)
            {
                var ord = GetDeliveryForOrder(or.Order_Id);
                delivery.Add(ord);
            }

            return delivery;

        }

        //Gets the orders dne by a delivery company
        public List<Order_Table> GetDeliveriesByCompany(int DeliveryID)
        {
            List<Order_Table> Orders = new List<Order_Table>();
            dynamic del = (from d in db.Order_Tables
                           where d.Delivery_Id.Equals(DeliveryID)
                           select d).ToList();
            foreach(Order_Table or in del)
            {
                Orders.Add(or);
            }
            return Orders;
        }

        //<-----Adding To Cart----->
        //Adds to cart
        public bool AddtoCart(int ClientId,int ProductID,int quantity,Decimal price)
        {
            var cart = (from c in db.Carts
                        where c.Client_Id.Equals(ClientId) && c.Product_Id.Equals(ProductID)
                        select c).FirstOrDefault();
            if(cart==null)
            {
                var newCart = new Cart()
                {
                    Client_Id = ClientId,
                    Product_Id = ProductID,
                    Quantity = quantity,
                    Price = price,
                };
                db.Carts.InsertOnSubmit(newCart);
                try
                {
                    db.SubmitChanges();
                    return true;
                }catch(Exception ex)
                {
                    ex.GetBaseException();
                    return false;
                }
            }
            else
            {
                //if item exists edit it
                return EditFromCart(ClientId, ProductID, quantity, price);
            }
        }

        //Edits an Item already in the cart
        public bool EditFromCart(int ClientId, int ProductID, int quantity, Decimal price)
        {
            var cart = GetCartItem(ClientId, ProductID);

            if(cart!=null)
            {
                cart.Quantity = quantity;
                cart.Price = price;
                try
                {
                    //update
                    db.SubmitChanges();
                    return true;
                }
                catch (IndexOutOfRangeException ex)
                {
                    ex.GetBaseException();
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        //Returns a single cart item
        public Cart GetCartItem(int ClientID, int Prod_Id)
        {
            Cart cart = new Cart();
            cart = (from c in db.Carts
                    where c.Client_Id.Equals(ClientID) && c.Product_Id.Equals(Prod_Id)
                    select c).FirstOrDefault();
            return cart;

        }

        //Returns all the items from a client
        public List<Cart> GetAllCartItemsForClient(int ClientID)
        {
            List<Cart> ShoppingCart = new List<Cart>();
            dynamic shop = (from s in db.Carts
                            where s.Client_Id.Equals(ClientID)
                            select s).ToList();
            foreach(Cart c in shop)
            {
                ShoppingCart.Add(c);
            }
            return ShoppingCart;
        }

        public List<Product> GetAllProductsInCart(int ClientID)
        { 
             dynamic cartitems = GetAllCartItemsForClient(ClientID);
            List<Product> products = new List<Product>();
            foreach (Cart c in cartitems)
            {
                var pro = GetProduct(c.Product_Id);
                products.Add(pro);
            }
            return products;
        }

        //Remove an item from the cart
        public bool RemoveFromCart(int ClientId, int ProdID)
        {
            Cart cart = GetCartItem(ClientId, ProdID);
            db.Carts.DeleteOnSubmit(cart);
            try
            {
                db.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        //Remove Everything from cart
        public bool ClearTheCart(int ClientID)
        {
            dynamic cartitems = GetAllCartItemsForClient(ClientID);
            bool Removed= true;
            foreach(Cart c in cartitems)
            {
                Removed = RemoveFromCart(c.Client_Id, c.Product_Id);

            }
            return Removed;
        }

        //<-----Moving from cart to Order----->
    }
}
