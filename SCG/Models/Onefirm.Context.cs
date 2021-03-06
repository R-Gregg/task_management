﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCG.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AttendeeGuestType> AttendeeGuestTypes { get; set; }
        public virtual DbSet<Attendee> Attendees { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<FavoriteProject> FavoriteProjects { get; set; }
        public virtual DbSet<GuestType> GuestTypes { get; set; }
        public virtual DbSet<KeyPerson> KeyPersons { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Priority> Priorities { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectSchedule> ProjectSchedules { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Rsvp> Rsvps { get; set; }
        public virtual DbSet<SharedProject> SharedProjects { get; set; }
        public virtual DbSet<TaskNote> TaskNotes { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<Touch> Touches { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<EngagementMember> EngagementMembers { get; set; }
        public virtual DbSet<EngagementNote> EngagementNotes { get; set; }
        public virtual DbSet<Engagement> Engagements { get; set; }
        public virtual DbSet<EngageType> EngageTypes { get; set; }
        public virtual DbSet<RssFeed> RssFeeds { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<MessageOfTheDay> MessageOfTheDays { get; set; }
    }
}
