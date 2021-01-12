using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataManager
{
    public class DataReader
    {
        static UvlotEntities uvDb = new UvlotEntities();
        UserRole userrole = new UserRole();


        public dynamic GetLoanApplication(string LoanID )
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                              
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               join i in uvDb.Banks on a.BankCode equals i.Code
                               join m in uvDb.EmployerLoanDetails on a.ID equals               m.LoanApplication_FK
                               join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.LoanRefNumber == LoanID
                               select new AppLoan
                               {
                                   ID = a.ID,
                                   MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   NigerianStates = f.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   LoanProduct = h.LoanProduct1,
                                   //ApplicationStatus = b.Description,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = i.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.HasValue? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft =    a.ExistingLoan_NoOfMonthsLeft.Value,
                               ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   Organization = a.Organization,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   Occupation = m.Occupation,
                                   Department = m.Department,
                                   Repayment = x.Name,
                                   LoanComment = a.LoanComment,
                                   EmployeeStatus = es.Name,
                               }).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




      /*  public LoanApplication GetLoanApplication(string LoanID)
        {
            try
            {
                var LoanRecord = (from a in uvDb.LoanApplications
                                  join 
                                  where a.LoanRefNumber == LoanID  
                                 select new LoanApplications
                                  {
                                      ID = 
                                      LoanRefNumber  =
                                      Title_FK =
                                      Surname =  
                                      Firstname =
                                      Othernames =
                                      Gender_FK =
                                      MaritalStatus_FK =
                                      MeansOfID_FK =
                                      IdentficationNumber = 
                                      DateOfBirth =
                                      PhoneNumber =
                                      EmailAddress =
                                      ContactAddress =
                                      Landmark =
                                      ClosestBusStop =
                                      StateofResidence_FK =
                                      LGA_FK =
                                      AccomodationType_FK =
                                      Organization =
                                      ApplicantID =
                                      NOK_FullName =
                                      NOK_Relationship =
                                      NOK_PhoneNumber =
                                      NOK_EmailAddress =
                                      NOK_HomeAddress =
                                      LoanAmount =
                                      LoanTenure =
                                      RepaymentMethod_FK =
                                      ExistingLoan =
                                      ExistingLoan_OutstandingAmount =
                                      ExistingLoan_NoOfMonthsLeft =
                                      Bank_FK =
                                      AccountNumber =
                                      AccountName =
                                      BVN =
                                      ValueDate =
                                      ValueTime =
                                      DateCreated =
                                      DateModified =
                                      ApplicationStatus_FK =
                                      LoanComment =
                                      IsVisible =
                                      CreatedBy =
                                      Recommend =
                                      StateofResidence =
                                      }).FirstOrDefault();
                if (LoanRecord == null)
                {
                    return null;
                }
                return LoanRecord;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        */
        
       public LoanApplication GetLoanApplicationold(string LoanID)
        {
            try
            {
                var LoanRecord = (from a in uvDb.LoanApplications where a.LoanRefNumber == LoanID select a).FirstOrDefault();
                if(LoanRecord == null)
                {
                    return null;




    }
                return LoanRecord;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool Validate(string value)
        {
            try
            {
                var user = (from a in uvDb.Users
                            where a.EmailAddress == value
                            select a).FirstOrDefault();
                if (user != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public bool ValidatePage(string value)
        {
            try
            {
                var pag = (from a in uvDb.Pages
                           where a.PageName == value
                           select a).FirstOrDefault();
                if (pag != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public User getUser(string email)
        {
            try
            {
                var users = (from a in uvDb.Users where a.EmailAddress == email select a).FirstOrDefault();

                if (users == null)
                {
                    return null;
                }

                return users;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }



        public dynamic LoanApRep()
        {
            try
            {
                dynamic MyDynamic = new System.Dynamic.ExpandoObject();
                var AppLoan = (from a in uvDb.LoanApplications
                               
                               where a.AccomodationType_FK == 1
                               select new
                               {

                                   AccountName = a.AccountName,
                                   LoanRefNum = a.LoanRefNumber,

                               }).ToList();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool loggedIn(string username, string password)
        {
            try
            {
                var Loggedin = (from a in uvDb.Users
                                where a.EmailAddress == username || a.PhoneNumber == username && a.PaswordVal == password
                                select a).FirstOrDefault();

                if (Loggedin != null)
                {
                    return true;
                }
                return false;
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }

        }


        public bool ValidateRole(string value)
        {
            try
            {
                var role = (from a in uvDb.Roles
                            where a.RoleName == value
                            select a).FirstOrDefault();
                if (role != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public List<User> getAllUser()
        {
            try
            {

                var user = (from a in uvDb.Users select a);

                if (user == null)
                {
                    return null;
                }
                return user.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public User getUserProfile(int id)
        {
            try
            {

                var users = (from a in uvDb.Users where a.ID == id select a).FirstOrDefault();
                if (users == null)
                {
                    return null;
                }
                return users;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string UpdateProfiles(User users)
        {
            try
            {
                var original = uvDb.Users.Find(users.EmailAddress);

                if (original != null)
                {
                    original.Firstname = users.Firstname;
                    original.Lastname = users.Lastname;
                    //original.EmailAddress = users.EmailAddress;
                    original.PhoneNumber = users.PhoneNumber;


                    uvDb.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Page> getPag(int id)
        {
            try
            {

                var pag = (from a in uvDb.Pages where a.PageID == id select a).ToList();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public Page getPage(int id)
        {
            try
            {

                var pag = (from a in uvDb.Pages where a.PageID == id select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public Page getPageByName(string pageName)
        {
            try
            {

                var pag = (from a in uvDb.Pages where a.PageName == pageName select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public User getUserByEmails(string Email)
        {
            try
            {
              var pag = (from a in uvDb.Users where a.EmailAddress == Email select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<Page> getAllPage()
        {
            try
            {

                var Pag = (from a in uvDb.Pages select a);

                if (Pag == null)
                {
                    return null;
                }
                return Pag.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        // I Added This Today
        public List<Page> getAllPages()
        {
            try
            {

                var Pag = (from a in uvDb.Pages select a).ToList();

                if (Pag == null)
                {
                    return null;
                }
                return Pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<getPagesx> getAllPag()
        {
            try
            {

                var Pag = (from a in uvDb.Pages 
                            select new getPagesx
                            { PageID = a.PageID, PageName = a.PageName.ToString(), IsVisible = a.IsVisible, ValueDate = a.ValueDate , PageDescription  = a.PageDescription, PageUrl = a.PageUrl}
                          ).ToList();
                
                if (Pag == null)
                {
                    return null;
                }
                return Pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string UpdatePages(Page pages)
        {
            try
            {
                var original = uvDb.Pages.Find(pages.PageName);

                if (original != null)
                {
                    original.PageUrl = pages.PageUrl;
                    original.PageDescription = pages.PageDescription;

                    uvDb.SaveChanges();
                }
                return original.PageID.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<LoanApplication> GetLoans()
        {
            try
            {
                var Loans = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == 3 select a ).ToList();

                if(Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<LoanApplication> ApproveLoans()
        {
            try
            {
                var Loans = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == 2 select a).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public Role getRole(int id)
        {
            try
            {

                var role = (from a in uvDb.Roles where a.RoleId == id select a).FirstOrDefault();
                if (role == null)
                {
                    return null;
                }
                return role;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<int> buildNamesList(User users)
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in uvDb.UserRoles where a.User_FK == users.ID select a.Role_FK).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        public List<int> buildAllRoleList()
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in uvDb.Roles select a.RoleId).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        public List<Role> getAllRole()
        {
            try
            {

                var role = (from a in uvDb.Roles select a);

                if (role == null)
                {
                    return null;
                }
                return role.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<GetAssignRoles> getUserRoles(int userid)
        {
            try
            {
                 var result = (from a in uvDb.UserRoles
                 join c in uvDb.Roles on a.Role_FK equals c.RoleId
                 join b in uvDb.Users on a.User_FK equals b.ID
                 where a.User_FK == userid
                 select new GetAssignRoles { userid = b.ID, Roleid = c.RoleId.ToString(), Rolename = c.RoleName, email = b.EmailAddress }).ToList();
                return result;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<GetAssignPages> getPageRoles(int pageid)
        {
            try
            {
                var result = (from a in uvDb.PageAuthentications
                              join c in uvDb.Roles on a.Role_FK equals c.RoleId
                              join b in uvDb.Pages on a.PageName equals b.PageName
                              //  where b.id == pages.id
                              where b.PageID == pageid
                              select new GetAssignPages { pageid = b.PageID, Roleid = c.RoleId.ToString(), Rolename = c.RoleName }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<int> buildPagesList(Page pages)
        {
            try
            {

                List<int> pageids = new List<int>();
                pageids = (from a in uvDb.PageAuthentications where a.PageName == pages.PageName select a.Role_FK.Value).ToList();

                if (pageids == null)
                {
                    return null;
                }
                return pageids;


            }
            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        public List<UnGetAssignRoles> getPageUrl(IEnumerable<int> newList){

            try
            {
                var pageurl = (from p in uvDb.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();
                return pageurl;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            }


        public List<getAllUserAndRoles> getAllUsersAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                User users = new DataAccessLayer.DataManager.User();
                LoanViewModel lvm = new LoanViewModel();
                string user = "tolutl@yahoo.com";
                users.EmailAddress = user;
                users.ID = selectUserID(users);

                var results = (from a in uvDb.UserRoles
                               join c in uvDb.Roles on a.Role_FK equals c.RoleId
                               join b in uvDb.Users on a.User_FK equals b.ID
                               //where a.UserId == users.id
                               select new getAllUserAndRoles { userid = b.ID, roleid = c.RoleId, rolename = c.RoleName, email = b.EmailAddress, id = a.ID }).ToList();
                var model = new LoanViewModel
                {
                    getAllUserAndRoless = results.ToList(),

                };
                lvm.getAllUserAndRoless = results;
                return lvm.getAllUserAndRoless.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int selectRolesByName(UserRole userrole)
        {

            try
            {
                var rol = (from a in uvDb.UserRoles where a.Role_FK == userrole.Role_FK && a.User_FK == userrole.User_FK select a).FirstOrDefault();

                if (rol == null)
                {
                    return 0;
                }


                int user = Convert.ToInt32(rol.Role_FK);
                int roleid = user;
                userrole.Role_FK = roleid;
                return user;
            }
            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
                return 0;
            }

        }


       
        public int selectUserID(User users)
        {

            try
            {
                var emails = (from a in uvDb.Users where a.EmailAddress == users.EmailAddress select a).FirstOrDefault();

                if (emails == null)
                {
                    return 0;
                }
                users.ID = Convert.ToInt16(emails.ID);
                return users.ID;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public string getSelectedPage(int value)
        {
            try
            {
                var page = (from a in uvDb.Pages
                            where a.PageID == value
                            select a).FirstOrDefault();

                if (page != null)
                {
                    return page.PageName;
                }
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string getSelectedEmail(int value)
        {
            try
            {
                var page = (from a in uvDb.Users
                            where a.ID == value
                            select a).FirstOrDefault();

                if (page != null)
                {
                    return page.EmailAddress;
                }
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<getAllPagesAndRoles> getAllPagesAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                LoanViewModel lvm = new LoanViewModel();
                var results = (from a in uvDb.PageAuthentications
                               join c in uvDb.Roles on a.Role_FK equals c.RoleId
                                
                               select new getAllPagesAndRoles

                               {
                                   id = a.ID,
                                   rolename = c.RoleName,
                                   roleid = c.RoleId,
                                   pageName = a.PageName,
                                  


                               }).ToList();


                var model = new LoanViewModel
                {
                    getAllPagesAndRoless = results.ToList(),

                };
                lvm.getAllPagesAndRoless = results;
                return lvm.getAllPagesAndRoless.ToList();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int selectPageRolesByName(PageAuthentication _pa)
        {

            try
            {
                var rol = (from a in uvDb.PageAuthentications where a.Role_FK == _pa.Role_FK && a.PageName == _pa.PageName select a).FirstOrDefault();

                if (rol == null)
                {
                    return 0;
                }


                int user = Convert.ToInt32(rol.Role_FK);
                int roleid = user;
                userrole.Role_FK = roleid;
                return user;
            }
            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public int getUserID(string email)
        {

            try
            {
         var userid = (from a in uvDb.Users where a.EmailAddress == email select a.ID).FirstOrDefault();

                if (userid == 0)
                {
                    return 0;
                }

                return userid;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public bool ValidateRole(List<Menus> menus ,string value)
        {
            try
            {
                var sirec = menus.Find(x => x.pageurl == value);

                if (sirec != null)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
        public List<AccountsModel> getUserByEmail(int id)
        {
            try
         {
                var user = (from a in uvDb.Users
                            where a.ID == id
                            select new AccountsModel
                            {
                                id = a.ID,
                                firstname = a.Firstname,
                                lastname = a.Lastname,
                                Phone = a.PhoneNumber,
                                Email = a.EmailAddress,
                            }).ToList();

                return user.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<int> getUserRols(int id)
        {
            try
            {
           var userroles = (from a in uvDb.UserRoles where a.User_FK == id select a.Role_FK).ToList();
                return userroles.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
           
        }

        public List<Role> getUserRoles(List<int> roleids)
        {
            //  here i am 
            try
            {
                var roles = (from p in uvDb.Roles where roleids.Contains((int)(p.RoleId)) select p).ToList();
                if (roles == null)
                {
                    return null;
                }

                return roles;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }


        public List<Menus> getResults(List<string> rol)
        {
            try
            {
                var results = (from p in uvDb.Pages
                               join pa in uvDb.PageAuthentications on p.PageName equals pa.PageName
                              // join ph in uvDb.pageHeaders on p.pageHeader equals ph.id
                               join r in uvDb.Roles on pa.Role_FK equals r.RoleId 

                               where rol.Contains(r.RoleName)
                               select new Menus
                               {
                                   pageName = pa.PageName,
                                   roleid = (int)pa.Role_FK,
                                   //pageheader = ph.page_header,
                                   pageurl = p.PageUrl,

                               }).Distinct();

                return results.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<Menus> GetAllMenu()
        {
            try
            {
                var results = (from p in uvDb.Pages
                               select new Menus
                 {
                  pageName = p.PageName,
                  roleid = (int)p.PageID,
                  pageurl = p.PageUrl,
                 }).Distinct();

                return results.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Bank> GetBanks()
        {
            try
            {
                var Services = (from a in uvDb.Banks select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }
        public List<LGA> GetLGAsByStateFK(int StateFK)
        {
            try
            {
                var Services = (from a in uvDb.LGAs select a)
                 .Where(x => x.State_FK == StateFK).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<RepaymentMethod> GetRepaymentMethods()
        {
            try
            {
                var Services = (from a in uvDb.RepaymentMethods select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<Title> GetTitles()
        {
            try
            {
                var Services = (from a in uvDb.Titles select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<MaritalStatu> GetMaritalStatus()
        {
            try
            {
                var Services = (from a in uvDb.MaritalStatus select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<LGA> GetAllLGAs()
        {
            try
            {
                var Services = (from a in uvDb.LGAs select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<AccomodationType> GetAccomodationTypes()
        {
            try
            {
                var Services = (from a in uvDb.AccomodationTypes select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<MeansOfIdentification> GetMeansOfIdentifications()
        {
            try
            {
                var Services = (from a in uvDb.MeansOfIdentifications select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<NigerianState> GetNigerianStates()
        {
            try
            {
                var Services = (from a in uvDb.NigerianStates select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public static dynamic GetBankNameByCode(string bankCode)
        {
            try
            {
                UvlotEntities dbU = new UvlotEntities();
                var bankObj = dbU.Banks.Where(x => x.IsActive == true && x.Code == bankCode).FirstOrDefault();
                return bankObj;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return "";
            }

        }


        public int GetLocalGovt(string LGA)
        {
            try
            {

                var Govt = (from a in uvDb.LGAs
                            where a.Name == LGA
                            select a).FirstOrDefault();
                if (Govt == null)
                {

                    return 0;
                }

                return Govt.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }


        

             public int GetMaritalStatus(string status)
           {
            try
            {

                var idname = (from a in uvDb.MaritalStatus
                              where a.Name == status
                              select a).FirstOrDefault();
                if (idname == null)
                {

                    return 0;
                }

                return idname.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetMeansofIdbyname(string MeansOfID)
        {
            try
            {

                var idname = (from a in uvDb.MeansOfIdentifications
                              where a.Name == MeansOfID
                              select a).FirstOrDefault();
                if (idname == null)
                {

                    return 0;
                }

                return idname.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public string getBankName(string value)
        {
            var Bank = (from a in uvDb.Banks where a.Code == value select a.Name).FirstOrDefault();
            if (Bank == null)
            {
                return null;
            }
            return Bank;
        }

        public string getTitleByID(int value)
        {
            try
            {
                var Title = (from a in uvDb.Titles where a.ID == value select a.Name).First();

                 if(Title == null)
                {
                    return null;
                }
                return Title;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string getStateByID(int value)
        {
            try
            {
                var Title = (from a in uvDb.NigerianStates where a.ID == value select a.Name).First();

                if (Title == null)
                {
                    return null;
                }
                return Title;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string getRepaymentByID(int value)
        {
            try
            {
                var Repay = (from a in uvDb.RepaymentMethods where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getMeanOfId(int value)
        {
            try
            {
                var Repay = (from a in uvDb.MeansOfIdentifications where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string MaritalStatus(int value)
        {
            try
            {
                var Repay = (from a in uvDb.MaritalStatus where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string Loantenure(int value)
        {
            try
            {
                var Repay = (from a in uvDb.LoanTenures where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public int GetTitleIDByName(string Tittle)
        {
            try
            {
                var TitleName = (from a in uvDb.Titles
                                 where a.Name == Tittle
                                 select a).FirstOrDefault();
                if (TitleName == null)
                {
                    return 0;
                }
                return TitleName.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int GetEmpoStatus(string Empostatus)
        {
            try
            {
                var Status = (from a in uvDb.EmploymentStatus
                              where a.Name == Empostatus
                              select a).FirstOrDefault();
                if (Status == null)
                {

                    return 0;
                }

                return Status.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int GetStatus(string MARITALSTATUS)
        {
            try
            {
                var Status = (from a in uvDb.MaritalStatus
                              where a.Name == MARITALSTATUS
                              select a).FirstOrDefault();
                if (Status == null)
                {

                    return 0;
                }

                return Status.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int GetAccomType(string ACCOMMODATIONTYPE)
        {
            try
            {
                var Accomodation = (from a in uvDb.AccomodationTypes
                                    where a.Name == ACCOMMODATIONTYPE
                                    select a).FirstOrDefault();
                if (Accomodation == null)
                {

                    return 0;
                }

                return Accomodation.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetState(string STATEOFRESIDENCE)
        {
            try
            {
                var state = (from a in uvDb.NigerianStates
                             where a.Name == STATEOFRESIDENCE
                             select a).FirstOrDefault();
                if (state == null)
                {

                    return 0;
                }

                return state.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        /*  public List<pageHeader> getAllPageHeader()
          {
              try
              {

                  var PagHeader = (from a in uvDb.pageHeaders select a);

                  if (PagHeader == null)
                  {
                      return null;
                  }
                  return PagHeader.ToList();
              }
              catch (Exception ex)
              {
                  return null;
              }
          }*/


    }

    public class UnGetAssignRoles
    {

        public int Roleid { get; set; }
        public string Rolename { get; set; }



    }

    public class GetAssignPages
    {
        public int pageid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }



    }

    public class getAllUserAndRoles
    {
        public int userid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string email { get; set; }
        public int id { get; set; }
    }


    public class getAllPagesAndRoles
    {
        // public int pageid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string pageName { get; set; }
        public int id { get; set; }

        public string pageUrl { get; set; }






    }


    public class GetAssignRoles
    {
        public int userid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }
        public string email { get; set; }


    }


    public class LoanViewModel
    {
        
        public User UserModel
        { get;
          set;
        }
        public Page PageModel
        {
            get;
            set;
        }

        public UserRole UserRole
        {
            get;
            set;

        }

       
        public IEnumerable<GetAssignRoles> GetAssignRoless
        {
            get;
            set;
        }
        public IEnumerable<UnGetAssignRoles> UnGetAssignRoless
        {
            get;
            set;
        }

       
        public IEnumerable<GetAssignPages> GetAssignPagess
        {
            get;
            set;
        }

        public IEnumerable<getAllUserAndRoles> getAllUserAndRoless
        {
            get;
            set;
        }

        public IEnumerable<getAllPagesAndRoles> getAllPagesAndRoless
        {
            get;
            set;
        }


        public IEnumerable<getPagesx> getPagesx
        {
            get;
            set;
        }

    }

    public class AccountsModel
    {

        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string Phone { get; set; }
        public string pasword { get; set; }
        public string confirmPassword { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string ResetPassword { get; set; }
        public DateTime DateTim { get; set; }
        public int isVissibles { get; set; }

        public string Referal { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }

     
    }

    public class Menus
    {
        public string pageName { get; set; }

        public int roleid { get; set; }

        public string pageheader { get; set; }

        public string pageurl { get; set; }


    }

    public class getPagesx
    {
      //  public IEnumerable<getPagesx> comp { get; set; }
        public int PageID { get; set; }
        public string PageName { get; set; }
        public Nullable<int> IsVisible { get; set; }
        public string ValueDate { get; set; }
        public string PageDescription { get; set; }
        public string PageUrl { get; set; }
    }


    public  class LoanApplications
    {
        public int ID { get; set; }
        public string LoanRefNumber { get; set; }
        public int Title_FK { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public int Gender_FK { get; set; }
        public int MaritalStatus_FK { get; set; }
        public int MeansOfID_FK { get; set; }
        public string IdentficationNumber { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ContactAddress { get; set; }
        public string Landmark { get; set; }
        public string ClosestBusStop { get; set; }
        public int StateofResidence_FK { get; set; }
        public int LGA_FK { get; set; }
        public int AccomodationType_FK { get; set; }
        public string Organization { get; set; }
        public string ApplicantID { get; set; }
        public string NOK_FullName { get; set; }
        public string NOK_Relationship { get; set; }
        public string NOK_PhoneNumber { get; set; }
        public string NOK_EmailAddress { get; set; }
        public string NOK_HomeAddress { get; set; }
        public Nullable<double> LoanAmount { get; set; }
        public int LoanTenure { get; set; }
        public int RepaymentMethod_FK { get; set; }
        public bool ExistingLoan { get; set; }
        public Nullable<double> ExistingLoan_OutstandingAmount { get; set; }
        public int ExistingLoan_NoOfMonthsLeft { get; set; }
        public int Bank_FK { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BVN { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public int ApplicationStatus_FK { get; set; }
        public string LoanComment { get; set; }
        public int IsVisible { get; set; }
        public string CreatedBy { get; set; }

        public int Recommend { get; set; }

        public string StateofResidence { get; set; }
    }


    public class AppLoan
    {
        public string Department { get; set; }

        public string Occupation { get; set; }
        public string AccountName { get; set; }
        public string LoanRefNumber { get; set; }
        public string LoanProduct { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string MeansOfIdentifications { get; set; }
        public string IdentficationNumber { get; set; }

        public string DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string ContactAddress { get; set; }

        public string Landmark { get; set; }

        public string ClosestBusStop { get; set; }

        public string NigerianStates { get; set; }

        public string LGAs { get; set; }

        public string Organization { get; set; }

        public string ApplicantID { get; set; }

        public string NOK_FullName { get; set; }

        public string NOK_Relationship { get; set; }

        public string NOK_PhoneNumber { get; set; }
        public string NOK_EmailAddress { get; set; }
        public string NOK_HomeAddress { get; set; }
        public Double LoanAmount { get; set; }
        public int LoanTenure { get; set; }
        public bool ExistingLoan { get; set; }
        public double ExistingLoan_OutstandingAmount { get; set; }
        public int ExistingLoan_NoOfMonthsLeft { get; set; }

        public string BankCode { get; set; }
        public string AccountNumber { get; set; }

        public string ValueTime { get; set; }
        public string ValueDate { get; set; }
        public string BVN { get; set; }
        public string ApplicationStatus { get; set; }

        public string Repayment { get; set; }

        public int ID { get; set; }
        public string LoanComment { get; set; }

        public string EmployeeStatus { get; set; }
    }






}
