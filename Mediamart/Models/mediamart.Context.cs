﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mediamart.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MediaMEntities : DbContext
    {
        public MediaMEntities()
            : base("name=MediaMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<A_Access_Center> A_Access_Center { get; set; }
        public virtual DbSet<A_Export_Log> A_Export_Log { get; set; }
        public virtual DbSet<A_Import_Items> A_Import_Items { get; set; }
        public virtual DbSet<Accessary_Big> Accessary_Big { get; set; }
        public virtual DbSet<Accessary_Export> Accessary_Export { get; set; }
        public virtual DbSet<Accessary_Import> Accessary_Import { get; set; }
        public virtual DbSet<Accessary_Key> Accessary_Key { get; set; }
        public virtual DbSet<Acessary_Export_Item> Acessary_Export_Item { get; set; }
        public virtual DbSet<Acessary_Log_Key> Acessary_Log_Key { get; set; }
        public virtual DbSet<Agent_Bonus> Agent_Bonus { get; set; }
        public virtual DbSet<Agent_Log_Active> Agent_Log_Active { get; set; }
        public virtual DbSet<Agent_Log_Payment> Agent_Log_Payment { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<B_Cate> B_Cate { get; set; }
        public virtual DbSet<B_Log_Active> B_Log_Active { get; set; }
        public virtual DbSet<B_Model> B_Model { get; set; }
        public virtual DbSet<B_Model_Process> B_Model_Process { get; set; }
        public virtual DbSet<B_Payment> B_Payment { get; set; }
        public virtual DbSet<B_Process> B_Process { get; set; }
        public virtual DbSet<B_Voucher> B_Voucher { get; set; }
        public virtual DbSet<Config_Bonus> Config_Bonus { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Log_Action> Log_Action { get; set; }
        public virtual DbSet<LOG_MO> LOG_MO { get; set; }
        public virtual DbSet<Model_Price> Model_Price { get; set; }
        public virtual DbSet<Move_Price> Move_Price { get; set; }
        public virtual DbSet<ProductCate> ProductCates { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Repair_Price> Repair_Price { get; set; }
        public virtual DbSet<SendCode> SendCodes { get; set; }
        public virtual DbSet<Type_Err> Type_Err { get; set; }
        public virtual DbSet<User_Device> User_Device { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<Warranti_Image> Warranti_Image { get; set; }
        public virtual DbSet<Warranti_Log> Warranti_Log { get; set; }
        public virtual DbSet<Warranti_Accessary> Warranti_Accessary { get; set; }
        public virtual DbSet<A_Import> A_Import { get; set; }
        public virtual DbSet<A_Export_Items> A_Export_Items { get; set; }
        public virtual DbSet<A_Station> A_Station { get; set; }
        public virtual DbSet<A_Export> A_Export { get; set; }
        public virtual DbSet<A_Revoke> A_Revoke { get; set; }
        public virtual DbSet<A_Revoke_Items> A_Revoke_Items { get; set; }
        public virtual DbSet<Article_Cate> Article_Cate { get; set; }
        public virtual DbSet<ProductCrack> ProductCracks { get; set; }
        public virtual DbSet<Warranti_TypeError> Warranti_TypeError { get; set; }
        public virtual DbSet<A_Revoke_Log> A_Revoke_Log { get; set; }
        public virtual DbSet<ProductCrack_Item> ProductCrack_Item { get; set; }
        public virtual DbSet<Warranti_Station> Warranti_Station { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Warranti> Warrantis { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Station_Level> Station_Level { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspModule> AspModules { get; set; }
        public virtual DbSet<AspModuleRole> AspModuleRoles { get; set; }
    }
}