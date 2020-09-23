using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Group_Project
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGroupService" in both code and config file together.
    [ServiceContract]
    public interface IGroupService
    {
        //user management
        //login function
        [OperationContract]
        int Login(string email, string password);

        //getting the user from the table
        [OperationContract]
        User_Table GetUser(int id);

        //getting email
        [OperationContract]
        User_Table GetEmail(string email, int id);

        //getting admin
        [OperationContract]
        Admin GetAdmin(int id);

        //getting client
        [OperationContract]
        Client GetClient(int id);

        //registering
        [OperationContract]
        string Register(string username, string password, string name, string email, string contactno, int active, string address, string surname = null, string businesstype = null, string usertype = "client");

        //updating the user
        [OperationContract]
        string UpdateInfo(string username, string name, string email, string contactno, string address, int id, string surname = null, string businesstype = null);

        //getting id
        [OperationContract]
        Mask_Type GetMask(int id);

        //change password
        [OperationContract]
        string Changepassword(string email, int id, string password);

        //hash password
        //active 
        //delete user
        [OperationContract]
        string DeleteUser(int id);


        [OperationContract]
        string AddGuest(int id, string names, string address, int orderid);



        //Invoice
        //get the invoice
        [OperationContract]
        Order_Table GetInvoice(int id);

        //getting the order items
        [OperationContract]
        Order_Item GetItem(int id);

        //get all items
        [OperationContract]
        List<Order_Item> Getallitems();

        //get all invoices
        [OperationContract]
        List<Order_Table> GetallInvoices();


        //get invoices by client id
        [OperationContract]
        List<Order_Table> GetInvoicebyclient(int clientid);

        //get invoices by date
        [OperationContract]
        List<Order_Table> GetInvoicebbydate(DateTime d);




        //Product catalog
        [OperationContract]
        string Addproducts(string name, string description, Decimal price, int active, int maskid, int admin, int quantity);

        [OperationContract]
        string Editproduct(string name, string description, Decimal price, int id, int active, int maskid, int admin, int quantity);

        [OperationContract]
        string Addtype(string name, string description, int admin);

        [OperationContract]
        string Edittype(string name, string description, int admin, int id);

        [OperationContract]
        string Addsize(string name, string dimensions);

        [OperationContract]
        string Editsize(string name, string dimen, int id);

        [OperationContract]
        string Addpsize(int sizeid, int psize);

        [OperationContract]
        string Updatepsize(int sizeid, int psize, int id);

        [OperationContract]
        string Addcustom(int filter, string size, string colour, string imageURL = "");

        [OperationContract]
        string Editcustom(int filter, string size, string colour, int _ID, string imageURL = "");

        [OperationContract]
        List<Product> Getallproducts();

        [OperationContract]
        List<Mask_Type> GetallMasktypes();

        [OperationContract]
        Product GetProduct(int id);

        //get custom
        [OperationContract]
        Custom_Product Getcustom(int id);

        //get size
        [OperationContract]
        Size_Table Getsize(int id);

        //get prod size from product table
        [OperationContract]
        Product_Size Getproductsize(int pid, int sid);

        //delete size, product size, product and customproduct, mask type
        [OperationContract]
        string Deletesize(int id);

        [OperationContract]
        string DeletePsize(int pid, int sid);

        [OperationContract]
        string DeleteMaskt(int id);

        [OperationContract]
        string Deletecustom(int id);

        [OperationContract]
        List<Product> GetProductsbycategory(int maskid);
        [OperationContract]
        string DeleteProduct(int P_Id);
        [OperationContract]
        List<Product> Getproductbyprice(decimal min, decimal max);
        [OperationContract]
        List<Product_Size> Getproductbysize(int sizeid);
        [OperationContract]
        Product_Size Getsproductsize(int sid);

      

        //Report


        //<---------------------------------------------Shopping Cart------------------------------------------------->
        //<-----Deliveries----->
        [OperationContract]
        List<Delivery> GetAllDeliveries();
        [OperationContract]
        List<Delivery> GetDeliveriesForClient(int ClientID);
        [OperationContract]
        List<Delivery> GetDeliveriesByCompany(int DeliveryID);
		[OperationContract]
		Delivery GetDeliveryForOrder(int orderID);
		[OperationContract]
		Delivery GetDelivery(int DeliveryID);
        //<-----Adding To Order----->
		bool AddtoCart(int ClientId,int ProductID,int quantity,Decimal price);
		[OperationContract]
		bool EditFromCart(int ClientId, int ProductID, int quantity, Decimal price);
		[OperationContract]
		Cart GetCartItem(int ClientID, int Prod_Id);
		[OperationContract]
		List<Cart> GetAllCartItemsForClient(int ClientID);
		[OperationContract]
		List<Product> GetAllProductsInCart(int ClientID);
		[OperationContract]
		bool RemoveFromCart(int ClientId, int ProdID);
		[OperationContract]
		bool ClearTheCart(int ClientID);
    }
}
