using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SyriaShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IsActive", "NameAr", "NameEn", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0f7e5ee2-66b5-4419-8833-9f36f7e14c8b"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "الإلكترونيات", "Electronics", "electronics", new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("1214d81b-c1b2-40ba-a55b-1145a2d0f691"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "الجمال والعناية", "Beauty & Care", "beauty-care", new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("59b730e4-b760-4e58-9cf5-e0b70e9b29b3"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "الطعام والمونة", "Food & Pantry", "food-pantry", new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("7d4ab6c0-1951-4a91-b6e5-ff0b19b65a22"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "المنزل والمطبخ", "Home & Kitchen", "home-kitchen", new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("8303e221-e4c7-4351-935a-6f4a906f4d0a"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "القرطاسية", "Stationery", "stationery", new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1e4a9dd-41d1-4b17-b8ab-f46bc0e14876"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "الملابس", "Clothing", "clothing", new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "DescriptionAr", "DescriptionEn", "ImageUrl", "IsActive", "NameAr", "NameEn", "Price", "Slug", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f01"), new Guid("0f7e5ee2-66b5-4419-8833-9f36f7e14c8b"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "سماعة مدمجة مع علبة شحن وبطارية تعمل حتى 20 ساعة.", "Compact earbuds with charging case and 20 hours of playtime.", "https://cdn.syrishop.example/earbuds.jpg", true, "سماعة بلوتوث لاسلكية", "Wireless Bluetooth Earbuds", 185000m, "wireless-earbuds", 40, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f02"), new Guid("0f7e5ee2-66b5-4419-8833-9f36f7e14c8b"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "قياس نبض القلب وتتبع النوم وهيكل مقاوم للماء.", "Heart rate, sleep tracking and waterproof case.", "https://cdn.syrishop.example/watch.jpg", true, "ساعة ذكية للرياضة", "Smart Fitness Watch", 450000m, "smart-watch-fit", 15, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f03"), new Guid("0f7e5ee2-66b5-4419-8833-9f36f7e14c8b"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "إضاءة ساطعة قابلة للضبط للبث والتصوير.", "Adjustable brightness ring light for streaming and photography.", null, true, "حلقة إضاءة LED", "LED Ring Light", 95000m, "led-ring-light", 30, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f04"), new Guid("c1e4a9dd-41d1-4b17-b8ab-f46bc0e14876"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "قطن 100% متنفس بثلاث ألوان.", "100% breathable cotton in three colors.", "https://cdn.syrishop.example/tshirt.jpg", true, "قميص قطني رجالي", "Cotton T-Shirt Men", 48000m, "cotton-tshirt-men", 120, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f05"), new Guid("c1e4a9dd-41d1-4b17-b8ab-f46bc0e14876"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "سترة صوف دافئة لأيام الشتاء الباردة.", "Warm wool jacket for cold winter days.", "https://cdn.syrishop.example/jacket.jpg", true, "سترة صوف شتوية", "Wool Winter Jacket", 320000m, "wool-winter-jacket", 35, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f06"), new Guid("c1e4a9dd-41d1-4b17-b8ab-f46bc0e14876"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "جينز متين بقصة عادية.", "Sturdy denim jeans with regular fit.", null, true, "بنطال جينز كلاسيكي", "Classic Denim Jeans", 180000m, "classic-jeans", 60, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f07"), new Guid("7d4ab6c0-1951-4a91-b6e5-ff0b19b65a22"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "طقم من عشر قطع بطلاء الجرانيت.", "Ten-piece cookware set with granite coating.", "https://cdn.syrishop.example/cookware.jpg", true, "طقم طهي 10 قطع", "Nonstick Cookware 10 Pieces", 275000m, "nonstick-cookware-10pc", 20, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f08"), new Guid("7d4ab6c0-1951-4a91-b6e5-ff0b19b65a22"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "غلاية سريعة الغليان مع إيقاف تلقائي.", "Fast boiling kettle with automatic shut-off.", null, true, "غلاية كهربائية 1 لتر", "Electric Kettle 1L", 135000m, "electric-kettle-1l", 50, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f09"), new Guid("7d4ab6c0-1951-4a91-b6e5-ff0b19b65a22"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "وسادة مريحة تدعم الرقبة والكتف.", "Ergonomic pillow that supports neck and shoulder.", null, true, "وسادة ميموري فوم", "Memory Foam Pillow", 65000m, "memory-foam-pillow", 80, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f0a"), new Guid("1214d81b-c1b2-40ba-a55b-1145a2d0f691"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "سيروم موحد للون البشرة بفيتامين سي وحمض الهيالورونيك.", "Brightening serum with vitamin C and hyaluronic acid.", "https://cdn.syrishop.example/serum.jpg", true, "سيروم فيتامين سي", "Vitamin C Serum", 52000m, "vitamin-c-serum", 90, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f0b"), new Guid("1214d81b-c1b2-40ba-a55b-1145a2d0f691"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "زيت أركان معصور على البارد لتنسيق طبيعي.", "Cold-pressed argan oil for natural styling.", null, true, "زيت الأركان للشعر", "Argan Hair Oil", 38000m, "argan-hair-oil", 110, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f0c"), new Guid("1214d81b-c1b2-40ba-a55b-1145a2d0f691"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "قناع تنظيف عميق بالفحم النشط.", "Deep cleansing mask with activated charcoal.", null, true, "قناع فحمي للوجه", "Charcoal Face Mask", 22000m, "charcoal-face-mask", 150, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f0d"), new Guid("59b730e4-b760-4e58-9cf5-e0b70e9b29b3"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "زيت زيتون بكر ممتاز من بساتين اللاذقية.", "Extra virgin olive oil from Latakia groves.", "https://cdn.syrishop.example/olive.jpg", true, "زيت زيتون ممتاز 1 لتر", "Premium Olive Oil 1L", 160000m, "premium-olive-oil-1l", 70, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f0e"), new Guid("59b730e4-b760-4e58-9cf5-e0b70e9b29b3"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "فستق مملح محمص على الطريقة اللاذقية.", "Salted pistachio roasted the Latakia way.", "https://cdn.syrishop.example/pistachio.jpg", true, "فستق محمص 500 غرام", "Roasted Pistachio 500g", 195000m, "roasted-pistachio-500g", 45, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f0f"), new Guid("59b730e4-b760-4e58-9cf5-e0b70e9b29b3"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "شاي سائب مزروع بدون مبيدات.", "Loose-leaf black tea grown without pesticides.", null, true, "شاي أسود عضوي 200 غرام", "Organic Black Tea 200g", 45000m, "organic-black-tea-200g", 100, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b3f1a4c0-97f2-41b2-8d6e-1c2a8e021f10"), new Guid("8303e221-e4c7-4351-935a-6f4a906f4d0a"), new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "192 صفحة مسطرة وورق سميك وإغلاق مطاطي.", "192 ruled pages, heavyweight paper, elastic closure.", null, true, "دفتر مقاس A5 بغلاف صلب", "Hardcover Notebook A5", 32000m, "hardcover-notebook-a5", 200, new DateTimeOffset(new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameAr",
                table: "Categories",
                column: "NameAr");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                column: "NameEn");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
