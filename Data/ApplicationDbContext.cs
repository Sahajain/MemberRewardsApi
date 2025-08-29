using MemberRewards.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace MemberRewards.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<MemberRewards.Models.Point> Points { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
    }
}