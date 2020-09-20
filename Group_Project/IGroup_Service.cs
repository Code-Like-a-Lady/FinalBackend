using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Group_Project
{
//Just a change 
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGroup_Service" in both code and config file together.
    [ServiceContract]
    public interface IGroup_Service
    {
        //user management
        //login function
        [OperationContract]
        int login(string email, string password);

        //getting the user from the table
        [OperationContract]
        User_Table getUser(int id);

        //getting email
        [OperationContract]
        User_Table getEmail(string email, int id);

        //getting admin
        [OperationContract]
        Admin getAdmin(int id);

        //getting client
        [OperationContract]
        Client getClient(int id);

        //registering
        [OperationContract]
        string Register(string username, string password, string name, string email, string contactno, int active, string address, string surname = null, string businesstype = null, string usertype = "client");

        //updating the user
        [OperationContract]
        string UpdateInfo(string username, string name, string email, string contactno, string address, int id, string surname = null, string businesstype = null);

        //getting id
        [OperationContract]
        Mask_Type getMask(int id);

        //change password
        [OperationContract]
        string Changepassword(string email, int id, string password);

        //hash password
        //active 
        //delete user
        [OperationContract]
        string deleteUser(int id);


        [OperationContract]
        string AddGuest(int id, string names, string address, int orderid);









        //Invoice
        //get the invoice
        [OperationContract]
        Order_Table getInvoice(int id);

        //getting the order items
        [OperationContract]
        Order_Item getItem(int id);

        //get all items
        [OperationContract]
        List<Order_Item> getallitems();

        //get all invoices
        [OperationContract]
        List<Order_Table> getallInvoices();


        //get invoices by client id
        [OperationContract]
        List<Order_Table> getInvoicebyclient(int clientid);

        //get invoices by date
        [OperationContract]
        List<Order_Table> getInvoicebbydate(DateTime d);








        //Product catalog
        [OperationContract]
        string addproducts(string name, string description, Decimal price, int active,int maskid, int admin, int quantity );

        [OperationContract]
        string editproduct(string name, string description, Decimal price,int id, int active, int maskid, int admin, int quantity);

        [OperationContract]
        string addtype(string name, string description, int admin);

        [OperationContract]
        string edittype(string name, string description, int admin,int id);

        [OperationContract]
        string addsize(string name, string dimensions);

        [OperationContract]
        string editsize(string name, string dimen, int id);

        [OperationContract]
        string addpsize(int sizeid, int psize);

        [OperationContract]
        string updatepsize(int sizeid, int psize, int id);

        [OperationContract]
        string addcustom(int pid, int filter, string size);

        [OperationContract]
        string editcustom(int pid, int filter, string size, int id);

        [OperationContract]
        List<Product> getallproducts();

        [OperationContract]
        List<Mask_Type> getallMasktypes();

        [OperationContract]
        Product getProduct(int id);

        //get custom
        [OperationContract]
        Custom_Product getcustom(int id);

        //get size
        [OperationContract]
        Size_Table getsize(int id);

        //get prod size from product table
        [OperationContract]
        Product_Size getproductsize(int pid, int sid);

        //delete size, product size, product and customproduct, mask type
        [OperationContract]
        string deletesize(int id);

        [OperationContract]
        string deletePsize(int pid,int sid);

        [OperationContract]
        string deleteMaskt(int id);

        [OperationContract]
        string deletecustom(int id);

        [OperationContract]
        List<Product> getProductsbycategory(int maskid);
        [OperationContract]
        string deleteProduct(int P_Id);
		[OperationContract]
		List<Product> getproductbyprice(decimal min, decimal max);
		[OperationContract]
		List<Product_Size> getproductbysize(int sizeid);
		[OperationContract]
		Product_Size getsproductsize(int sid);
        //Report


        //<---------------------------------------------Shopping Cart------------------------------------------------->
        //<-----Deliveries----->
        [OperationContract]
        List<Delivery> GetAllDeliveries();
        [OperationContract]
        List<Delivery> GetDeliveriesForClient(int ClientID);
        [OperationContract]
        List<Delivery> GetDeliveriesByCompany(int DeliveryID);
        //<-----Adding To Order----->
       // [OperationContract]
        //int AddOrderItem();
        

    }

}
