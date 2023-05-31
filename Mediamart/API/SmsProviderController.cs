using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Mediamart.Models;
using Mediamart.Utils;

namespace Mediamart.API
{
    [RoutePrefix("api/sms")]
    public class SmsProviderController : ApiController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        MediaMEntities db = new MediaMEntities();
        [Route("receive")]
        [HttpGet]
        public HttpResponseMessage Recevice(string Command_Code, string User_ID, string Service_ID, string Request_ID, string Message)
        {
            //https://localhost:44339/api/sms/receive?command_code=OLIVO&user_id=0984654998&request_id=1&service_id=1&message=OLIVO%20MK%20
            string MT = saicuphap;
            int status = 0;
            logger.Info(string.Format("[MO] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, Message));
            try
            {

                //format sdt ve dau 0
                User_ID = Utils.Control.formatUserId(User_ID, 2);
                string[] words = Message.ToUpper().Trim().Split(' ');
                if (words[0].Equals("OLIVO"))
                {
                    if (words.Length == 2)
                    {
                        if (words[1] == "BH")//bao loi
                        {
                            var customer = db.Customers.SingleOrDefault(a => a.Phone == User_ID);
                            if (customer == null)
                            {
                                MT = bh_notvalid;
                            }
                            else
                            {
                                //luu thong tin bảo hành
                                CreateWarranti(customer);
                                MT = bh_ok;
                            }
                        }
                        else if (words[1] == "MK")//Lấy OTP
                        {
                            var customer = db.Customers.SingleOrDefault(a => a.Phone == User_ID);

                            var sendCode = db.SendCodes.SingleOrDefault(a => a.Code == User_ID);
                            if (customer == null)
                            {
                                MT = get_otp_phone_notvalid;
                            }
                            else
                            {
                                //tao ma xac nhan
                                string PIN = Utils.Control.CreatePIN();
                                //gui cho sdt
                                string statusOtp = "0";
                                bool check = false;
                                var sendcode = new SendCode()
                                {
                                    Phone = customer.Phone,
                                    Code = PIN,
                                    Createdate = DateTime.Now,
                                    SendStatus = int.Parse(statusOtp),
                                    CheckStatus = check,
                                };
                                db.SendCodes.Add(sendcode);
                                db.SaveChanges();

                                MT = string.Format(get_otp, sendcode.Code);

                                CheckOTP(statusOtp);
                            }
                        }
                        else//kích hoạt thường
                        {
                            string code = words[1];
                            var product = db.Products.SingleOrDefault(a => a.Code == code);
                            if (product == null)
                            {
                                MT = kh_notvalid;
                            }
                            else if (product.Active_date != null)
                            {
                                MT = string.Format(kh_actived, product.Serial, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                //neu so kich hoat la so dai ly=> thong bao
                                //comment để k check sdt đại lý
                                var _agent = db.AspNetUsers.Where(a => a.PhoneNumber == User_ID);
                                if (_agent.Count() > 0)
                                {
                                    MT = phone_agent;
                                }
                                else
                                {
                                    var old_custromer = db.Customers.SingleOrDefault(a => a.Phone == User_ID);
                                    if (old_custromer == null)
                                    {
                                        var customer = new Customer()
                                        {
                                            Phone = User_ID,
                                            Createdate = DateTime.Now,
                                            Chanel = "SMS"
                                        };
                                        db.Customers.Add(customer);
                                        db.SaveChanges();
                                    }
                                    product.Active_phone = User_ID;
                                    product.Status = 1;
                                    product.Active_date = DateTime.Now;
                                    product.End_date = DateTime.Now.AddMonths(product.Limited);
                                    product.Active_chanel = "SMS";
                                    db.Entry(product).State = EntityState.Modified;
                                    db.SaveChanges();


                                    CreateActive(product, User_ID, "");
                                    MT = string.Format(kh_ok, product.Serial, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"));
                                }
                            }
                        }
                    }
                    else if (words.Length == 3)
                    {
                        if (words[1] == "TC")//tra cuu bao hanh
                        {
                            string code = words[2];
                            var product = db.Products.SingleOrDefault(a => a.Code == code);
                            var today = DateTime.Now;
                            today = today.AddDays(1);
                            if (product == null || product.Active_date == null)
                            {
                                //khong ton tai serial
                                MT = seri_notvalid;
                            }
                            else if (product.End_date < today)
                            {
                                //qua han bao hanh
                                MT = string.Format(seri_outdate, product.Code, product.End_date.Value.ToString("dd/MM/yyyy"));
                            }
                            else if (product.Active_date != null)
                            {
                                MT = string.Format(seri_actived, product.Code, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"));
                            }
                        }
                        else//khong dung cu phap
                        {

                            //thêm trường hợp kích hoạt có tên khách hàng
                            string code = words[1];
                            string name = words[2];
                            var product = db.Products.SingleOrDefault(a => a.Code == code);
                            if (product == null)
                            {
                                MT = seri_notvalid;
                            }
                            else if (product.Active_date != null)
                            {
                                MT = string.Format(kh_actived, product.Code, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                CreateActive(product, User_ID, name);

                                MT = string.Format(seri_actived, product.Code, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"));
                            }
                        }
                    }
                    else if (words.Length == 4)
                    {
                        string code = words[1];
                        string sdt_khachhang = words[2];
                        string user = words[3];
                        //sdt khách hàng không đúng=> tin nhắn k đúng cú pháp
                        sdt_khachhang = Utils.Control.formatUserId(sdt_khachhang, 2);
                        if (Utils.Control.getMobileOperator(sdt_khachhang) == "UNKNOWN")
                        {
                            MT = phone_notallow;
                        }
                        else
                        {
                            //mã đại lý không tồn tại
                            var agent = db.AspNetUsers.SingleOrDefault(a => a.UserName == user);
                            if (agent == null)
                            {
                                MT = agent_notvalid;
                            }
                            else
                            {
                                var product = db.Products.SingleOrDefault(a => a.Code == code);
                                if (product == null)
                                {
                                    MT = kh_notvalid;
                                }
                                else if (product.Active_date != null)
                                {
                                    MT = string.Format(kh_actived, product.Serial, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"));
                                }
                                else if (sdt_khachhang.Equals(User_ID))
                                {
                                    MT = agent_notallow;
                                }
                                else
                                {
                                    //neu so kich hoat la so dai ly=> thong bao
                                    //var _agent = db.AspNetUsers.Where(a => a.PhoneNumber == sdt_khachhang);
                                    //if (_agent.Count() > 0)
                                    //{
                                    //    MT = phone_agent;
                                    //}
                                    //else
                                    //{
                                        var old_custromer = db.Customers.SingleOrDefault(a => a.Phone == sdt_khachhang);
                                        if (old_custromer == null)
                                        {
                                            var customer = new Customer()
                                            {
                                                Phone = sdt_khachhang,
                                                Createdate = DateTime.Now,
                                                Chanel = "SMS"
                                            };
                                            db.Customers.Add(customer);
                                            db.SaveChanges();
                                        }
                                        //dai ly kich hoat
                                        product.Active_by = agent.UserName;
                                        //khach  hang tu kich hoat
                                        product.Active_phone = sdt_khachhang;
                                        product.Status = 1;
                                        product.Active_date = DateTime.Now;
                                        product.End_date = DateTime.Now.AddMonths(product.Limited);
                                        product.Active_chanel = "SMS";
                                        product.Active_code = User_ID;
                                        db.Entry(product).State = EntityState.Modified;
                                        db.SaveChanges();

                                        AgentActive(product, agent.UserName, sdt_khachhang);

                                        MT = string.Format(agent_ok, product.Serial, product.Active_date.Value.ToString("dd/MM/yyyy"), product.End_date.Value.ToString("dd/MM/yyyy"), agent.UserName);
                                    //}
                                }
                            }
                        }
                    }
                    else//khong dung cu phap
                    {
                        MT = saicuphap;
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
                MT = saicuphap;
                logger.Error(ex.Message);
            }

            logger.Info(MT);

            var result = new Result()
            {
                status = "0",
                message = MT,
                phone = User_ID
            };
            var log_mo = new LOG_MO()
            {
                User_Id = User_ID,
                Service_Id = Service_ID,
                Request_Id = Request_ID,
                Message = Message,
                Createdate = DateTime.Now,
                Status = status,
                Response = MT
            };
            db.LOG_MO.Add(log_mo);
            db.SaveChanges();
            //var response = new HttpResponseMessage();
            //response.Content = new StringContent("0|" + MT);
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            logger.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, MT));
            var response = new HttpResponseMessage();
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            return response;
        }



        Product AgentActive(Product product, string agent, string sdt_khachhang)
        {
            var old_customer = db.Customers.SingleOrDefault(a => a.Phone == sdt_khachhang);
            if (old_customer == null)
            {
                old_customer = new Customer()
                {
                    Phone = sdt_khachhang,
                    Createdate = DateTime.Now,
                    Chanel = "SMS"
                };
                db.Customers.Add(old_customer);
                db.SaveChanges();
            }
            //dai ly kich hoat
            product.Active_by = agent;
            //khach  hang tu kich hoat
            product.Active_phone = sdt_khachhang;
            product.Status = 1;
            product.Active_date = DateTime.Now;
            product.End_date = DateTime.Now.AddMonths(product.Limited);
            product.Active_chanel = "SMS";
            db.Entry(product).State = EntityState.Modified;

            //thưởng kích hoạt cho đại lý tài khoản có phải đại lý không
            if (ConfigControl.isBonusActive() == true)
            {
                //log thưởng cho đại lý theo sản phẩm
                var agent_active = new Agent_Log_Active()
                {
                    Createdate = DateTime.Now,
                    Model = product.Model,
                    Product = product.Code,
                    UserName = product.Active_by,
                    Amount = Getpricemodel(product.Model)
                };
                db.Agent_Log_Active.Add(agent_active);
                //check role
                var _agency = db.AspNetUsers.SingleOrDefault(a => a.UserName == agent);
                if (_agency != null)
                {
                    var role = _agency.AspNetRoles.FirstOrDefault().Id;
                    if (role == "Agent")
                    {
                        //topup luôn
                        logger.Info(string.Format("SMS ACTIVE TOPUP @Phone{0} @Amount{1}", _agency.PhoneNumber, agent_active.Amount));
                        string TOPUP = Utils.TOPUP.TopuptoUserId(_agency.PhoneNumber, agent_active.Amount.ToString());
                        logger.Info(TOPUP);
                        //lưu lại thông tin 
                        var _bonus = db.Agent_Bonus.SingleOrDefault(a => a.UserName == product.Active_by);
                        if (_bonus != null)
                        {
                            _bonus.Used = _bonus.Used + agent_active.Amount;
                            _bonus.Active = _bonus.Active + agent_active.Amount;
                            _bonus.Newdate = DateTime.Now;
                            db.Entry(_bonus).State = EntityState.Modified;
                        }
                        else
                        {
                            var agent_bonus = new Agent_Bonus()
                            {
                                UserName = product.Active_by,
                                Used = agent_active.Amount,
                                Active = agent_active.Amount,
                                Createdate = DateTime.Now,
                                Newdate = DateTime.Now
                            };
                            db.Agent_Bonus.Add(agent_bonus);
                        }
                    }
                    else if (role == "Nhân viên")
                    {
                        //lưu thông tin lại
                        //cộng tiền vào tài khoản cho staff
                        var _bonus = db.Agent_Bonus.SingleOrDefault(a => a.UserName == product.Active_by);
                        if (_bonus != null)
                        {
                            _bonus.Active = _bonus.Active + agent_active.Amount;
                            _bonus.Newdate = DateTime.Now;
                            db.Entry(_bonus).State = EntityState.Modified;
                        }
                        else
                        {
                            var agent_bonus = new Agent_Bonus()
                            {
                                UserName = product.Active_by,
                                Active = agent_active.Amount,
                                Createdate = DateTime.Now,
                                Newdate = DateTime.Now
                            };
                            db.Agent_Bonus.Add(agent_bonus);
                        }
                    }
                }

            }
            //thưởng cho khách hàng
            AddBonus(product.Model, product.Code, old_customer);
            db.SaveChanges();
            return product;
        }
        Product CreateActive(Product product, string User_ID, string name)
        {
            var customer = db.Customers.SingleOrDefault(a => a.Phone == User_ID);
            if (customer == null)
            {
                customer = new Customer()
                {
                    Name = name,
                    Phone = User_ID,
                    Createdate = DateTime.Now,
                    Chanel = "SMS"
                };
                db.Customers.Add(customer);
                db.SaveChanges();
            }
            product.Active_phone = User_ID;
            product.Status = 1;
            product.Active_date = DateTime.Now;
            product.End_date = DateTime.Now.AddMonths(product.Limited);
            product.Active_chanel = "SMS";
            db.Entry(product).State = EntityState.Modified;
            //them chuong trinh khuyen mai
            AddBonus(product.Model, product.Code, customer);
            db.SaveChanges();
            return product;
        }
        void AddBonus(string Model, string ProductCode, Customer customer)
        {
            if (!string.IsNullOrEmpty(Model))
            {
                var bonus = db.B_Model_Process.Where(a => a.Model == Model && a.Status == true);
                if (bonus.Count() > 0)
                {
                    foreach (var item in bonus)
                    {
                        var process = db.B_Process.SingleOrDefault(a => a.Id == item.Process && a.Status == true);
                        if (process != null)
                        {
                            if (process.Startdate != null && process.Enddate != null)
                            {
                                //check hạn sử dụng chương trình
                                if (process.Startdate.Value.Date > DateTime.Now.Date)
                                {
                                    //chưa đến ngày được tham gia chương trình
                                    continue;
                                }
                                if (process.Enddate.Value.Date < DateTime.Now.Date)
                                {
                                    //hết ngày tham gia chương trình
                                    continue;
                                }
                            }
                            var log = new B_Log_Active()
                            {
                                Model = Model,
                                Process = process.Name,
                                ProcessId = item.Process,
                                ProductCode = ProductCode,
                                Unit = process.Unit,
                                Quantity = process.Quantity
                            };
                            db.B_Log_Active.Add(log);
                            if (process.Name == "TD")
                            {
                                customer.PointActive = customer.PointActive + process.Quantity;
                            }
                        }
                    }
                    //db.SaveChanges();
                }
            }

        }
        void CreateWarranti(Customer customer)
        {
            var warranti = new Warranti()
            {
                Status = 0,
                Createdate = DateTime.Now,
                Chanel = "SMS",
                Phone = customer.Phone,
                Address = customer.Address,
                Ward = customer.Ward,
                District = customer.District,
                Province = customer.Province

            };
            db.Warrantis.Add(warranti);
            db.SaveChanges();
            //tao ma phieu bao hanh 
            warranti.Code = Utils.Control.CreateCode(warranti.Id);
            db.Entry(warranti).State = EntityState.Modified;
            db.SaveChanges();
        }
        int Getpricemodel(string Model)
        {
            var model = db.Model_Price.SingleOrDefault(a => a.Model == Model);
            if (model != null)
            {
                return model.Price;
            }
            else
            {
                return 0;
            }
        }
        void CheckOTP(string checkCode)
        {
            
        }
        class Result
        {
            public string message { get; set; }
            public string status { get; set; }
            public string phone { get; set; }
        }

        string saicuphap = "Cu phap khong dung, vui long soan OLIVO MaCao gui 8077. Moi thong tin lien he 1900638382. Tran trong cam on.";
        string seri_notvalid = "Ma cao san pham cua ban khong ton tai, vui long kiem tra lai. Moi thong tin lien he 1900638382 hoac https://baohanh.olivo.com.vn. Tran trong cam on.";
        string seri_outdate = "San pham {0} da het han bao hanh vao {1}. Moi thong tin lien he 1900638382 hoac https://baohanh.olivo.com.vn. Tran trong cam on.";
        string seri_actived = "San pham {0} da duoc kich hoat bao hanh thanh cong tu ngay {1} den ngay {2}. Moi thong tin lien he 1900638382. Tran trong cam on.";
        string bh_ok = "Yeu cau bao hanh cua ban da duoc ghi nhan, chung toi se lien he lai trong vong 24h. Lien he 1900638382 hoac https://baohanh.olivo.com.vn. Tran trong cam on.";
        string bh_notvalid = "So dien thoai Yeu cau bao hanh cua ban khong ton tai. Moi thong tin lien he 1900638382 hoac https://baohanh.olivo.com.vn. Tran trong cam on.";
        string kh_actived = "San pham {0} da kich hoat tu ngay {1} den ngay {2}. Moi thong tin lien he 1900638382. Tran trong cam on.";
        string kh_ok = "San pham {0} da duoc kich hoat thanh cong tu ngay {1} den ngay {2}. Moi thong tin lien he 1900638382. Tran trong cam on.";
        string kh_notvalid = "Ma so cao cua ban khong ton tai, vui long kiem tra lai. Moi thong tin lien he 1900638382. Tran trong cam on.";

        string phone_notallow = "Kich hoat khong thanh cong, So dien thoai Khach hang khong dung dinh dang cua nha mang. Moi thong tin lien he 1900638382.Tran trong cam on.";
        string phone_agent = "Kich hoat khong thanh cong, So dien thoai Khach Hang trung voi so dien thoai Dai ly. Moi thong tin lien he 1900638382.Tran trong cam on.";
        
        string agent_notvalid = "TU CHOI do ma dai ly khong ton tai trong he thong. Moi thong tin lien he 1900638382. Tran trong cam on.";
        
        string agent_ok = "San pham {0} kich hoat thanh cong tai {3} vao ngay {1}, ngay het han {2}. Moi thong tin lien he 1900638382. Tran trong cam on.";
        string agent_notallow = "Kich hoat khong thanh cong, So dien thoai Khach Hang trung voi so dien thoai Kich hoat. Moi thong tin lien he 1900638382.Tran trong cam on.";

        string get_otp = "Ma OTP cua ban la: {0}, OTP de reset mat khau tai https://baohanh.olivo.com.vn va co thoi gian 30 phut. Moi thong tin lien he 1900638382. Tran trong cam on.";
        string get_otp_phone_notvalid = "SDT chua dang ky tai khoan thanh vien OLIVO tai https://baohanh.olivo.com.vn. Moi thong tin lien he 1900638382.Tran trong cam on.";
    }
}
