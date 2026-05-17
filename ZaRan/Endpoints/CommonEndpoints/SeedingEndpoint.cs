using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.CommonEndpoints;

public class SeedingEndpoint(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher
) : EndpointWithoutRequest<Results<Ok<string>, BadRequest<string>>>
{
    public override void Configure()
    {
        Post("/seeding");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<string>, BadRequest<string>>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            // 检查是否已经有数据
            if (await dbContext.Users.AnyAsync(cancellationToken: ct))
            {
                return TypedResults.BadRequest("数据库已包含数据，无需重复初始化");
            }

            // 创建用户数据
            var users = await CreateUsers();

            // 创建文章数据
            var articles = await CreateArticles(users);

            // 创建文章集合数据
            var collections = await CreateArticleCollections(users, articles);

            // 创建商品数据
            var goods = await CreateGoods(users, articles);

            // 创建消息数据
            var messages = await CreateMessages(users);

            await dbContext.SaveChangesAsync(cancellationToken: ct);

            return TypedResults.Ok(
                $"数据初始化成功！创建了 {users.Count} 个用户，{articles.Count} 篇文章，{collections.Count} 个集合，{goods.Count} 个商品，{messages.Count} 条消息");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"数据初始化失败: {ex.Message}");
        }
    }

    private async Task<List<User>> CreateUsers()
    {
        var users = new List<User>
        {
            new User
            {
                UserName = "admin",
                NickName = "管理员",
                Email = "admin@zaranlabs.com",
                UserRole = UserRole.Admin,
                UserStatus = UserStatus.Normal,
                Balance = 10000.0,
                Signature = "檐下风铃平台管理员",
                CreatedAt = DateTimeOffset.Now,
                LastLoginAt = DateTimeOffset.Now,
                HashedPassword = passwordHasher.HashPassword(null!, "admin123")
            },
            new User
            {
                UserName = "zhaoshewen",
                NickName = "赵设纹",
                Email = "zhaoshewen@example.com",
                UserRole = UserRole.Publisher,
                UserStatus = UserStatus.Normal,
                Balance = 5000.0,
                Signature = "传统扎染工艺传承人，专注于云水纹设计",
                CreatedAt = DateTimeOffset.Now.AddDays(-30),
                LastLoginAt = DateTimeOffset.Now.AddDays(-1),
                HashedPassword = passwordHasher.HashPassword(null!, "password123")
            },
            new User
            {
                UserName = "liwenchuang",
                NickName = "李纹创",
                Email = "liwenchuang@example.com",
                UserRole = UserRole.Publisher,
                UserStatus = UserStatus.Normal,
                Balance = 3200.0,
                Signature = "现代扎染设计师，擅长几何图案创新",
                CreatedAt = DateTimeOffset.Now.AddDays(-25),
                LastLoginAt = DateTimeOffset.Now.AddDays(-2),
                HashedPassword = passwordHasher.HashPassword(null!, "password123")
            },
            new User
            {
                UserName = "wangyifang",
                NickName = "王艺坊",
                Email = "wangyifang@example.com",
                UserRole = UserRole.Publisher,
                UserStatus = UserStatus.Normal,
                Balance = 2800.0,
                Signature = "家居扎染用品设计专家",
                CreatedAt = DateTimeOffset.Now.AddDays(-20),
                LastLoginAt = DateTimeOffset.Now.AddDays(-3),
                HashedPassword = passwordHasher.HashPassword(null!, "password123")
            },
            new User
            {
                UserName = "chenranmo",
                NickName = "陈染墨",
                Email = "chenranmo@example.com",
                UserRole = UserRole.Publisher,
                UserStatus = UserStatus.Normal,
                Balance = 4500.0,
                Signature = "扎染艺术画创作者，山水意境专家",
                CreatedAt = DateTimeOffset.Now.AddDays(-18),
                LastLoginAt = DateTimeOffset.Now.AddDays(-1),
                HashedPassword = passwordHasher.HashPassword(null!, "password123")
            },
            new User
            {
                UserName = "qianchuangyi",
                NickName = "钱创艺",
                Email = "qianchuangyi@example.com",
                UserRole = UserRole.Publisher,
                UserStatus = UserStatus.Normal,
                Balance = 1800.0,
                Signature = "DIY扎染教学专家，几何拼接纹样设计师",
                CreatedAt = DateTimeOffset.Now.AddDays(-15),
                LastLoginAt = DateTimeOffset.Now.AddDays(-2),
                HashedPassword = passwordHasher.HashPassword(null!, "password123")
            },
            new User
            {
                UserName = "sunchuanyi",
                NickName = "孙传艺",
                Email = "sunchuanyi@example.com",
                UserRole = UserRole.Publisher,
                UserStatus = UserStatus.Normal,
                Balance = 3600.0,
                Signature = "传统花卉纹样传承人",
                CreatedAt = DateTimeOffset.Now.AddDays(-12),
                LastLoginAt = DateTimeOffset.Now.AddDays(-1),
                HashedPassword = passwordHasher.HashPassword(null!, "password123")
            }
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();
        return users;
    }

    private async Task<List<Article>> CreateArticles(List<User> users)
    {
        var articles = new List<Article>();

        // IP故事文章 - 扎染艺术故事背景
        articles.Add(new Article
        {
            Title = "扎染艺术故事",
            Type = ArticleType.Story,
            Status = ArticleStatus.Published,
            Body = """
                   ## 背景故事

                   ### 千年染灵起源
                   唐代盐商云集的自贡，染娘苏氏创
                   "八技封灵术"—将毕生执念注入代表不同技法的纹样，化作守护染艺的灵体。唯有苏氏血脉能唤醒染灵，但每代传承者需以心血供养灵体，否则将遭染疾反噬。

                   ### 青蓝坊的现代困境
                   - **空间侵占：** 祖传靛蓝灵泉被开发商填埋，建度假村
                   - **技艺断代：** 母亲去世，盐夏对"干叠冰纹染"心有排斥
                   - **染灵危机：** 近十年无人施展完整古法，染灵陷入濒死沉睡

                   村里的人们都说，风铃的技艺来自于她与自然的深度对话。每当夜深人静时，她会坐在檐下，聆听风铃的声音，那些细微的音符仿佛在告诉她染料的秘密、纹样的灵魂。

                   然而，随着时代的变迁，传统的扎染技艺面临着前所未有的挑战。现代化的浪潮冲击着这个古老的村落，年轻人纷纷离开，寻求更加现代的生活方式。风铃意识到，如果不采取行动，这些珍贵的技艺将会永远消失。

                   于是，她开始了一场关于传承与创新的旅程。她不仅要保护传统的扎染技艺，还要让它们在现代社会中重新焕发生机。这就是檐下风铃平台诞生的故事——一个连接传统与现代、守护与创新的数字文化平台。
                   """,
            Summary = "探索千年扎染技艺背后的传奇人物与守护灵，感受传统工艺与现代困境的碰撞。",
            Tags = ["IP故事", "传说", "扎染", "传承", "檐下风铃"],
            Color = "#3A7BD5",
            ImageUrl = "/image/IMG_8838.JPG",
            ImageSmallUrl = "/image/IMG_8838.JPG",
            AuthorId = users.First(u => u.UserName == "admin").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-10),
            UpdatedAt = DateTimeOffset.Now.AddDays(-10)
        });

        // 角色故事 - 盐夏
        articles.Add(new Article
        {
            Title = "盐夏：青蓝坊的传承者",
            Type = ArticleType.Character,
            Status = ArticleStatus.Published,
            Body = """
                   # 盐夏 · 青蓝坊的传承者

                   > "每一道蓝痕都是活着的记忆，而我，不过是染缸里浸泡太久的布料"

                   ## 人物档案

                   ### 身份
                   自贡百年扎染世家"青蓝坊"唯一传承者，美院染织专业毕业生

                   ### 外貌特征
                   素色棉麻衣衫，长发用龍蓝扎染布条束起，指尖常年染有洗不净的蓝痕

                   ### 性格特质
                   - **坚忍：** 独自支撑濒临倒闭的青蓝坊，每日工作16小时维持生计
                   - **细腻：** 能通过布料肌理判断染料配比，对色彩极度敏感

                   ### 专业技能
                   - 掌握217种古法植物染料配方（可凭气味辨别发酵程度）
                   - 独创"呼吸染法"：通过调节呼吸频率控制布料吸色均匀度

                   ### 致命缺陷
                   因心结无法施展家传绝技"千叠血引"

                   ## 心结与秘密

                   盐夏内心深处认为母亲因扎染技艺积劳成疾而死，对家传秘技具有复杂矛盾情感。这种情感阻碍了她完全掌握家族最高技艺"千叠血引"。

                   更不为人知的是，家族女性继承者血脉中潜伏着"染疾"，25岁后身体会逐渐结晶化。这种病症需要染灵之力压制，但随着染灵力量衰弱，盐夏的症状已经开始提前显现。

                   ## 与绞缬的关系

                   盐夏与染灵首领绞缬有着微妙的共生契约关系。染灵靠传承者的信念存续，而传承者则需要染灵的力量压制染疾。

                   最初，盐夏对这位千年染灵充满戒备，但随着时间推移，她逐渐理解绞缬守护传统技艺的执着。然而，她对施展需要消耗生命力的高阶技艺仍心存抗拒。

                   ## 当前困境

                   青蓝坊面临着三重危机：祖传靛蓝灵泉被开发商填埋；母亲去世导致高阶技艺传承断裂；近十年无人施展完整古法，导致染灵力量衰弱。

                   盐夏必须在染疾全面发作前找到解决办法，否则不仅会失去生命，千年传承的扎染技艺也将永远消失。
                   """,
            Summary = "青蓝坊唯一传承者，背负家族技艺传承重任和染疾诅咒的22岁少女",
            Tags = ["IP故事", "角色", "盐夏", "传承者", "染疾", "青蓝坊"],
            Color = "#87a9ed",
            ImageUrl = "/image/yanxia.jpg",
            ImageSmallUrl = "/image/yanxia1.jpg",
            AuthorId = users.First(u => u.UserName == "admin").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-9),
            UpdatedAt = DateTimeOffset.Now.AddDays(-9)
        });

        // 角色故事 - 绞缬
        articles.Add(new Article
        {
            Title = "绞缬：千叠冰纹染守护灵",
            Type = ArticleType.Character,
            Status = ArticleStatus.Published,
            Body = """
                   # 绞缬 · 千叠染灵

                   > "我们不是染料与布料的幽灵，而是千年不散的匠心"

                   ## 灵体档案

                   ### 身份
                   千叠冰纹染守护灵，沉睡千年的染灵首领

                   ### 本源构成
                   诞生于唐代宗广德元年（公元763年），由12名染娘临终执念凝聚

                   ### 外形特征
                   九色鹿形态，鹿角纹路随情绪变化，周身环绕淡淡靛蓝光晕

                   ### 特殊能力
                   感知扎染技艺的传承状态，引导其他染灵苏醒，消耗自身灵力延缓盐夏的染疾发作

                   ### 行为特征
                   厌恶机械复制品（靠近化纤布料会打喷嚏），月圆夜需浸泡在靛蓝草汁中维持形态，说谎时鹿角纹路会打结

                   ## 千年起源

                   唐代盐商云集的自贡，染娘苏氏创"八技封灵术"—将毕生执念注入代表不同技法的纹样，化作守护染艺的灵体。绞缬作为其中最强大的染灵，由十二位技艺精湛的染娘在生命最后时刻共同创造。

                   这些染娘因长期接触染料和过度劳作而身患重病，却仍心系技艺传承。她们的执念与未完成的作品融合，化作了这只九色鹿形态的染灵。

                   ## 与盐夏的关系

                   绞缬与盐夏家族有着千年的契约关系。唯有苏氏血脉能唤醒染灵，但每代传承者需以心血供养灵体，否则将遭染疾反噬。

                   绞缬对这位年轻的传承者既期待又担忧。它欣赏盐夏的天赋，却也为她对高阶技艺的抗拒而焦虑。随着盐夏染疾症状的提前显现，绞缬不得不消耗更多灵力来延缓这一过程，导致自身形态变得不稳定。

                   ## 现代危机

                   机械复制的扎染产品稀释了手艺的灵性，导致染灵力量不断衰弱。近十年无人施展完整古法，使得绞缬和其他染灵陷入濒死沉睡。

                   更严重的是，青蓝坊祖传的靛蓝灵泉被开发商填埋，建成了度假村。这处灵泉不仅是染灵力量的源泉，也是压制染疾的关键。失去灵泉后，绞缬必须在月圆之夜寻找替代的靛蓝草汁，否则将无法维持形态。
                   """,
            Summary = "唐代诞生的九色鹿染灵，由12名染娘执念凝聚，守护千叠冰纹染技艺",
            Tags = ["IP故事", "角色", "绞缬", "守护灵", "九色鹿", "唐代", "染灵"],
            Color = "#4b6cb7",
            ImageUrl = "/image/jiaoxie.jpg",
            ImageSmallUrl = "/image/jiaoxie.jpg",
            AuthorId = users.First(u => u.UserName == "admin").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-8),
            UpdatedAt = DateTimeOffset.Now.AddDays(-8)
        });

        // 扎染工艺知识文章
        articles.Add(new Article
        {
            Title = "扎染工艺的历史与发展",
            Type = ArticleType.Wiki,
            Status = ArticleStatus.Published,
            Body = """
                   扎染是中国传统的手工印染技艺，有着悠久的历史和深厚的文化底蕴。

                   **历史起源：**
                   扎染技艺起源于秦汉时期，在唐代达到鼎盛。据史料记载，唐代的绞缬技艺已经相当成熟，不仅在宫廷中广泛使用，民间也有大量的工匠从事这一行业。

                   **主要技法：**
                   1. **绞缬技法**：通过绳线绞扎布料，形成防染效果，创造出独特的纹样
                   2. **夹缬技法**：使用木板夹住布料进行染色，可制作复杂图案
                   3. **蜡缬技法**：用蜡作为防染剂的染色方法，线条流畅自然

                   **工艺流程：**
                   - 设计纹样：根据需要设计扎染图案
                   - 扎结布料：按照设计用绳线扎结布料
                   - 调制染料：配制天然植物染料
                   - 浸染过程：将扎好的布料浸入染缸
                   - 晾晒定色：自然晾晒，使颜色固定
                   - 拆线整理：拆除绳线，整理成品

                   **文化价值：**
                   扎染不仅是一种染色技艺，更承载着深厚的文化内涵。每一件扎染作品都是独一无二的，体现了手工艺的珍贵价值和文化传承的重要意义。
                   """,
            Summary = "详细介绍扎染工艺的历史发展、主要技法和文化价值",
            Tags = ["扎染", "工艺", "历史", "传统", "绞缬", "技法"],
            Color = "#00A896",
            ImageUrl = "/image/IMG_8828.JPG",
            ImageSmallUrl = "/image/IMG_8828.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-7),
            UpdatedAt = DateTimeOffset.Now.AddDays(-7)
        });

        // 纹样基因库文章
        articles.Add(new Article
        {
            Title = "云水纹的设计原理",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   云水纹是扎染中的经典纹样，其设计融合了自然元素，体现了中国传统美学的精髓。

                   **设计理念：**
                   云水纹的设计灵感来源于自然界中云朵的飘逸和水流的动态。通过扎染技艺，将这种自然的美感转化为布料上的艺术图案，既保持了传统技艺的精髓，又展现了自然之美。

                   **制作技法：**
                   1. **扎结技巧**：采用螺旋式扎结方法，从中心向外逐渐扩散
                   2. **染料配比**：使用天然靛蓝染料，通过多次浸染形成深浅层次
                   3. **时间控制**：每次浸染时间需精确控制，形成自然的渐变效果
                   4. **拆线技巧**：按照特定顺序拆除绳线，保持纹样的完整性

                   **艺术特色：**
                   - 纹样流畅自然，如云似水
                   - 色彩层次丰富，深浅有致
                   - 每件作品独一无二，体现手工艺价值
                   - 寓意吉祥，象征着生活的流畅和美好

                   **应用范围：**
                   云水纹广泛应用于服装、家居用品、艺术品等领域，是最受欢迎的扎染纹样之一。
                   """,
            Summary = "云水纹扎染纹样的设计原理、制作技法和艺术特色详解",
            Tags = ["云水纹", "纹样", "设计", "基因库", "传统", "自然"],
            Color = "#F7B733",
            ImageUrl = "/image/IMG_9056.JPG",
            ImageSmallUrl = "/image/IMG_9056.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-6),
            UpdatedAt = DateTimeOffset.Now.AddDays(-6)
        });

        articles.Add(new Article
        {
            Title = "几何拼接纹样创新设计",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   现代几何拼接纹样结合传统扎染技艺，创造出独特的视觉效果，是传统工艺与现代设计理念完美融合的典型代表。

                   **设计特点：**
                   几何拼接纹样打破了传统扎染的圆形、螺旋形等自然形态，采用直线、三角形、菱形等几何元素，通过规律性的排列组合，形成现代感强烈的图案效果。

                   **创新技法：**
                   1. **模块化扎结**：将布料分割成规则的几何区域，分别进行扎结
                   2. **多色染制**：使用多种颜色的染料，创造丰富的色彩层次
                   3. **精确控制**：通过精确的测量和计算，确保图案的规整性
                   4. **拼接组合**：将不同的几何模块进行创意组合

                   **应用创新：**
                   - 现代服装设计中的几何图案
                   - 家居装饰品的现代化改造
                   - 艺术品创作的新思路
                   - 文创产品的设计元素

                   **设计理念：**
                   几何拼接纹样体现了'传统技艺，现代表达'的设计理念，既保持了扎染技艺的核心特色，又满足了现代审美的需求，是传统工艺现代化发展的重要方向。
                   """,
            Summary = "几何拼接纹样的创新设计方法、技法特点和现代应用",
            Tags = ["几何", "拼接", "创新", "现代", "设计", "纹样"],
            Color = "#E85A4F",
            ImageUrl = "/image/IMG_9057.JPG",
            ImageSmallUrl = "/image/IMG_9057.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-5),
            UpdatedAt = DateTimeOffset.Now.AddDays(-5)
        });

        articles.Add(new Article
        {
            Title = "传统花卉纹样的现代演绎",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   传统花卉纹样在现代扎染中的创新应用和设计理念，展现了传统文化与现代审美的完美结合。

                   **传统花卉纹样特色：**
                   传统花卉纹样以牡丹、莲花、菊花、梅花等为主要元素，寓意富贵、纯洁、高雅、坚韧等美好品质。这些纹样在传统扎染中通过精巧的扎结技法，形成层次丰富、寓意深刻的图案。

                   **现代演绎手法：**
                   1. **简化设计**：保留花卉的基本形态，简化细节，适应现代简约风格
                   2. **色彩创新**：在传统蓝白基础上，增加更多现代色彩搭配
                   3. **尺寸调整**：根据现代产品需求，调整纹样的大小和比例
                   4. **组合创新**：将不同花卉元素进行创新组合，形成新的图案

                   **技法改进：**
                   - 采用更精确的扎结工具，提高纹样的精细度
                   - 结合现代染料技术，增强色彩的稳定性和丰富性
                   - 运用数字化设计，预先规划纹样效果

                   **应用领域：**
                   - 现代服装设计中的装饰元素
                   - 家居纺织品的图案设计
                   - 文创产品的主题纹样
                   - 艺术品创作的灵感来源

                   传统花卉纹样的现代演绎，既保持了传统文化的精神内核，又满足了现代生活的实用需求。
                   """,
            Summary = "传统花卉纹样的现代化设计理念、演绎手法和应用领域",
            Tags = ["花卉", "传统", "现代", "演绎", "纹样", "创新"],
            Color = "#8E44AD",
            ImageUrl = "/image/IMG_9058.JPG",
            ImageSmallUrl = "/image/IMG_9058.JPG",
            AuthorId = users.First(u => u.UserName == "sunchuanyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-4),
            UpdatedAt = DateTimeOffset.Now.AddDays(-4)
        });

        // 更多纹样基因库文章
        articles.Add(new Article
        {
            Title = "青旋螺纹",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   青旋螺纹是四川传统扎染中的经典纹样，以其层层叠叠的螺旋形态象征着生命的轮回与无限可能。

                   **设计特色：**
                   青旋螺纹采用从中心点向外扩散的螺旋式扎结技法，形成如漩涡般的动态美感。每一圈螺纹都有着不同的深浅层次，营造出立体的视觉效果。

                   **制作工艺：**
                   1. **中心定位**：精确确定布料的中心点作为螺纹起始位置
                   2. **螺旋扎结**：从中心开始，按照黄金比例进行螺旋式绑扎
                   3. **层次控制**：通过调节绳线的松紧度控制染色的深浅层次
                   4. **渐变处理**：多次浸染形成自然的色彩渐变效果

                   **文化内涵：**
                   青旋螺纹在传统文化中寓意着生生不息、循环往复的生命哲学，体现了中国古代对自然规律的深刻理解。

                   **现代应用：**
                   广泛应用于现代服装设计、家居装饰和艺术创作中，是最具代表性的扎染纹样之一。
                   """,
            Summary = "经典四川螺旋纹，层层叠叠，象征着生命的轮回与无限可能",
            Tags = ["螺旋纹", "传统", "四川", "青旋", "基因库"],
            Color = "#4A90E2",
            ImageUrl = "/image/IMG_8823.JPG",
            ImageSmallUrl = "/image/IMG_8823.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-8),
            UpdatedAt = DateTimeOffset.Now.AddDays(-8)
        });

        articles.Add(new Article
        {
            Title = "祥云漫卷",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   祥云漫卷纹样源自自贡的传统云纹，线条流畅飘逸，寓意吉祥如意，是扎染艺术中的经典之作。

                   **设计理念：**
                   祥云纹样的设计灵感来源于天空中飘逸的云朵，通过扎染技艺将云的轻盈和流动感完美地表现在布料上。

                   **技法特点：**
                   1. **流线扎结**：采用不规则的流线型扎结方法，模拟云朵的自然形态
                   2. **渐变染色**：使用多层次的染色技法，形成云朵的立体感
                   3. **留白处理**：巧妙运用留白技巧，增强云朵的飘逸感
                   4. **边缘柔化**：特殊的边缘处理技法，使纹样更加自然

                   **艺术价值：**
                   祥云纹样不仅具有很高的装饰价值，更承载着深厚的文化内涵，象征着吉祥、如意和美好的愿望。

                   **传承发展：**
                   从传统的单色云纹发展到现代的多彩云纹，祥云漫卷在传承中不断创新，适应现代审美需求。
                   """,
            Summary = "源自自贡的传统云纹，线条流畅飘逸，寓意吉祥如意",
            Tags = ["祥云", "云纹", "传统", "自贡", "吉祥"],
            Color = "#87CEEB",
            ImageUrl = "/image/IMG_8824.JPG",
            ImageSmallUrl = "/image/IMG_8824.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-7),
            UpdatedAt = DateTimeOffset.Now.AddDays(-7)
        });

        articles.Add(new Article
        {
            Title = "涟漪轻语",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   涟漪轻语是传统水波纹的精致演绎，细腻展现水的柔美动态，富有诗意与宁静感。

                   **设计灵感：**
                   取材于湖面微风轻拂时产生的层层涟漪，通过扎染技艺将这种自然现象转化为永恒的艺术图案。

                   **制作技法：**
                   1. **同心圆扎结**：以多个同心圆的方式进行扎结，模拟水波的扩散效果
                   2. **渐进染色**：从中心向外逐渐减淡颜色，形成自然的波纹效果
                   3. **细节处理**：精细的扎结技巧确保每一圈波纹都清晰可见
                   4. **动态表现**：通过不同的扎结密度表现水波的动态美感

                   **美学特色：**
                   涟漪轻语纹样具有很强的韵律感和节奏感，给人以宁静致远的美感体验。

                   **应用场景：**
                   特别适合用于禅意空间的装饰，如茶室、书房等，营造宁静祥和的氛围。
                   """,
            Summary = "传统水波纹，细腻展现水的柔美动态，富有诗意与宁静感",
            Tags = ["水波纹", "涟漪", "传统", "宁静", "诗意"],
            Color = "#B0E0E6",
            ImageUrl = "/image/IMG_8826.JPG",
            ImageSmallUrl = "/image/IMG_8826.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-6),
            UpdatedAt = DateTimeOffset.Now.AddDays(-6)
        });

        articles.Add(new Article
        {
            Title = "曜日之花",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   曜日之花是热情奔放的太阳花纹样，象征光明、活力与希望，融合了多元文化的设计元素。

                   **文化背景：**
                   这一纹样融合了非洲太阳崇拜文化和中国传统花卉纹样，体现了文化交流与融合的美好愿景。

                   **设计特点：**
                   1. **放射状结构**：以太阳为中心的放射状花瓣设计
                   2. **鲜艳色彩**：使用明亮的暖色调，表现太阳的热情与活力
                   3. **层次丰富**：多层次的花瓣结构，增强视觉冲击力
                   4. **动感表现**：通过不对称的设计增加动感

                   **制作工艺：**
                   采用特殊的多点扎结技法，结合渐变染色，形成太阳花的立体效果。

                   **象征意义：**
                   曜日之花象征着光明战胜黑暗，希望战胜绝望，是积极向上的生活态度的体现。

                   **现代价值：**
                   在现代设计中，常用于表达青春活力和积极向上的主题。
                   """,
            Summary = "热情奔放的太阳花纹样，象征光明、活力与希望",
            Tags = ["太阳花", "民族", "活力", "希望", "多元文化"],
            Color = "#FFD700",
            ImageUrl = "/image/IMG_8827.JPG",
            ImageSmallUrl = "/image/IMG_8827.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-5),
            UpdatedAt = DateTimeOffset.Now.AddDays(-5)
        });

        articles.Add(new Article
        {
            Title = "蝶舞翩跹",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   蝶舞翩跹是自贡特色蝴蝶纹，色彩斑斓，寓意爱情美满、生活幸福，展现了生命的美好与自由。

                   **设计理念：**
                   以蝴蝶的优美形态为设计灵感，通过扎染技艺表现蝴蝶翩翩起舞的动态美感。

                   **技法创新：**
                   1. **对称扎结**：采用对称的扎结方法，形成蝴蝶翅膀的对称美
                   2. **多色染制**：使用多种颜色的染料，表现蝴蝶翅膀的绚丽色彩
                   3. **细节刻画**：精细的扎结技巧表现蝴蝶翅膀的纹理细节
                   4. **动态表现**：通过不规则的边缘处理表现蝴蝶飞舞的动感

                   **文化寓意：**
                   蝴蝶在中国传统文化中象征着美好的爱情和幸福的生活，寓意着破茧成蝶的人生蜕变。

                   **艺术特色：**
                   色彩丰富、形态优美，具有很强的装饰性和观赏性。

                   **应用领域：**
                   广泛应用于女性服饰、儿童用品和婚庆装饰等领域。
                   """,
            Summary = "自贡特色蝴蝶纹，色彩斑斓，寓意爱情美满、生活幸福",
            Tags = ["蝴蝶纹", "民族", "自贡", "色彩", "美好"],
            Color = "#FF69B4",
            ImageUrl = "/image/IMG_8828.JPG",
            ImageSmallUrl = "/image/IMG_8828.JPG",
            AuthorId = users.First(u => u.UserName == "sunchuanyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-4),
            UpdatedAt = DateTimeOffset.Now.AddDays(-4)
        });

        articles.Add(new Article
        {
            Title = "鱼跃龙门",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   鱼跃龙门是四川传统鱼纹，象征年年有余、吉祥喜庆，也寓意着努力拼搏、追求成功的精神。

                   **文化背景：**
                   源自中国古代"鲤鱼跳龙门"的传说，寓意通过努力奋斗实现人生的飞跃和成功。

                   **设计特色：**
                   1. **流线造型**：以鱼的流线型身体为基础，表现鱼儿游动的优美姿态
                   2. **跳跃动感**：通过特殊的扎结技法表现鱼儿跳跃的瞬间
                   3. **鳞片纹理**：精细的扎结技巧模拟鱼鳞的质感
                   4. **水波背景**：结合水波纹样，营造鱼儿在水中游动的场景

                   **制作工艺：**
                   采用复合扎结技法，先制作鱼的主体形状，再添加鳞片和水波等细节。

                   **象征意义：**
                   在传统文化中，鱼象征着富裕和丰收，"年年有余"的美好寓意深受人们喜爱。

                   **现代应用：**
                   常用于节庆装饰、礼品设计和吉祥图案的创作。
                   """,
            Summary = "四川传统鱼纹，象征年年有余、吉祥喜庆，也寓意着努力拼搏",
            Tags = ["鱼纹", "传统", "四川", "吉祥", "年年有余"],
            Color = "#4169E1",
            ImageUrl = "/image/IMG_8829.JPG",
            ImageSmallUrl = "/image/IMG_8829.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-3),
            UpdatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        articles.Add(new Article
        {
            Title = "格致新语",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   格致新语是现代几何方格纹样，融入精细工艺，简约而不失雅致，体现了现代设计美学。

                   **设计理念：**
                   以"格物致知"的哲学思想为指导，通过简洁的几何形式表达深刻的文化内涵。

                   **技法特点：**
                   1. **精确测量**：使用精确的测量工具确保几何图案的规整性
                   2. **模块化设计**：将复杂图案分解为简单的几何模块
                   3. **色彩搭配**：运用现代色彩理论，创造和谐的视觉效果
                   4. **工艺精细**：每一个细节都经过精心设计和制作

                   **美学价值：**
                   体现了"少即是多"的现代设计理念，通过简洁的形式表达丰富的内容。

                   **文化融合：**
                   将传统扎染技艺与现代设计理念完美结合，创造出具有时代特色的新纹样。

                   **应用前景：**
                   适合现代简约风格的装饰需求，在建筑、室内设计等领域有广阔应用前景。
                   """,
            Summary = "现代几何方格，融入精细工艺，简约而不失雅致",
            Tags = ["几何", "现代", "方格", "简约", "精细"],
            Color = "#708090",
            ImageUrl = "/image/IMG_8825.JPG",
            ImageSmallUrl = "/image/IMG_8825.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-2),
            UpdatedAt = DateTimeOffset.Now.AddDays(-2)
        });

        articles.Add(new Article
        {
            Title = "山水印象",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   山水印象是传统山水画风格的扎染纹样，展现中国水墨画的意境，将绘画艺术与染织工艺完美结合。

                   **艺术理念：**
                   借鉴中国传统山水画的构图和意境，通过扎染技艺表现山水的空灵与深远。

                   **技法特色：**
                   1. **层次渲染**：模拟水墨画的浓淡层次，形成远山近水的空间感
                   2. **留白艺术**：巧妙运用留白技巧，营造云雾缭绕的意境
                   3. **渐变处理**：通过渐变染色表现山峦的起伏和水的流动
                   4. **意境营造**：注重整体意境的营造，追求"神似"而非"形似"

                   **文化内涵：**
                   体现了中国传统文化中"天人合一"的哲学思想和对自然的敬畏与热爱。

                   **现代价值：**
                   在快节奏的现代生活中，山水印象纹样能够带给人们宁静致远的心境。
                   """,
            Summary = "传统山水画风格的扎染纹样，展现中国水墨画的意境",
            Tags = ["山水", "传统", "水墨", "意境", "中国画"],
            Color = "#2F4F4F",
            ImageUrl = "/image/IMG_8831.JPG",
            ImageSmallUrl = "/image/IMG_8831.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        articles.Add(new Article
        {
            Title = "民族图腾",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   民族图腾融合多个少数民族图腾元素的现代设计作品，展现了中华民族文化的多样性和包容性。

                   **设计背景：**
                   汇集了彝族、苗族、藏族等多个民族的传统图腾元素，通过现代设计手法进行重新诠释。

                   **元素融合：**
                   1. **彝族太阳纹**：象征光明和希望的太阳图案
                   2. **苗族银饰纹**：精致繁复的银饰图案元素
                   3. **藏族祥云纹**：庄严神圣的宗教艺术符号
                   4. **壮族织锦纹**：色彩绚烂的传统织锦图案

                   **制作工艺：**
                   采用复合扎结技法，将不同的图腾元素有机结合，形成和谐统一的整体效果。

                   **文化意义：**
                   体现了中华民族大家庭的团结和谐，展现了多元文化的交流与融合。

                   **现代应用：**
                   适用于文化交流、民族团结主题的设计项目。
                   """,
            Summary = "融合多个少数民族图腾元素的现代设计作品",
            Tags = ["民族", "图腾", "多元文化", "融合", "传统"],
            Color = "#8B4513",
            ImageUrl = "/image/IMG_8832.JPG",
            ImageSmallUrl = "/image/IMG_8832.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now,
            UpdatedAt = DateTimeOffset.Now
        });

        articles.Add(new Article
        {
            Title = "星空幻想",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   星空幻想是现代抽象风格纹样，灵感来自浩瀚星空与宇宙，体现了人类对未知世界的探索精神。

                   **创作灵感：**
                   以夜空中的星座、银河、星云等天体现象为设计灵感，通过抽象的表现手法营造神秘的宇宙氛围。

                   **技法创新：**
                   1. **点状扎结**：使用点状扎结技法模拟星星的闪烁效果
                   2. **渐变染色**：通过深浅渐变表现宇宙的深邃与神秘
                   3. **随机分布**：采用随机分布的设计手法，模拟星空的自然状态
                   4. **光影效果**：通过特殊的染色技法营造星光的效果

                   **艺术特色：**
                   具有强烈的现代感和科幻色彩，给人以无限遐想的空间。

                   **象征意义：**
                   代表着人类对未知世界的好奇心和探索精神，体现了科学与艺术的完美结合。

                   **应用场景：**
                   适合用于科技主题、未来主义风格的设计项目。
                   """,
            Summary = "现代抽象风格，灵感来自浩瀚星空与宇宙",
            Tags = ["现代", "抽象", "星空", "宇宙", "科幻"],
            Color = "#191970",
            ImageUrl = "/image/IMG_8833.JPG",
            ImageSmallUrl = "/image/IMG_8833.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now,
            UpdatedAt = DateTimeOffset.Now
        });

        // 更多传统纹样
        articles.Add(new Article
        {
            Title = "像素浪潮",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   像素浪潮是用户创作的现代像素风格波浪纹，将传统元素与数字艺术结合，展现了传统工艺的现代化表达。

                   **设计理念：**
                   将传统的水波纹样与现代数字艺术的像素风格相结合，创造出具有强烈现代感的新纹样。

                   **技法特色：**
                   1. **像素化处理**：将传统波浪纹进行像素化分解，形成方块状的视觉效果
                   2. **数字化设计**：运用计算机辅助设计，精确控制每个像素点的位置
                   3. **色彩渐变**：通过不同深浅的蓝色营造波浪的层次感
                   4. **现代工艺**：结合传统扎染与现代印染技术

                   **文化意义：**
                   代表着传统工艺在数字时代的创新发展，体现了传统与现代的完美融合。

                   **应用前景：**
                   特别适合年轻群体和科技主题的设计项目，在游戏、动漫等领域有广泛应用。
                   """,
            Summary = "用户创作的现代像素风格波浪纹，将传统元素与数字艺术结合",
            Tags = ["像素", "现代", "波浪", "数字艺术", "创新"],
            Color = "#00CED1",
            ImageUrl = "/image/IMG_8830.JPG",
            ImageSmallUrl = "/image/IMG_8830.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        articles.Add(new Article
        {
            Title = "古韵新篇",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   古韵新篇是传统纹样与现代设计语言的完美结合，体现了传承与创新的和谐统一。

                   **设计理念：**
                   在保持传统纹样精神内核的基础上，运用现代设计手法进行重新诠释和表达。

                   **创新要素：**
                   1. **简化线条**：将复杂的传统纹样进行简化，突出主要特征
                   2. **现代色彩**：运用现代色彩搭配理论，增强视觉冲击力
                   3. **几何重构**：用几何化的手法重新构建传统图案
                   4. **空间布局**：运用现代平面设计的空间布局原理

                   **文化价值：**
                   既保持了传统文化的深厚底蕴，又满足了现代审美的需求，是文化传承的典型范例。

                   **应用领域：**
                   广泛应用于现代家居、文创产品、品牌设计等领域。
                   """,
            Summary = "传统纹样与现代设计语言的完美结合",
            Tags = ["古韵", "现代", "传统", "创新", "设计"],
            Color = "#9370DB",
            ImageUrl = "/image/IMG_9016.JPG",
            ImageSmallUrl = "/image/IMG_9016.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-2),
            UpdatedAt = DateTimeOffset.Now.AddDays(-2)
        });

        articles.Add(new Article
        {
            Title = "云山叠翠",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   云山叠翠是层叠山峦与流动云雾的渐变扎染，模仿水墨皴法，营造深远空间感的传统山水纹样。

                   **艺术特色：**
                   借鉴中国传统山水画的皴法技巧，通过扎染工艺表现山峦的层次和云雾的飘逸。

                   **制作技法：**
                   1. **层次扎结**：采用多层次的扎结方法，表现山峦的远近层次
                   2. **渐变染色**：通过渐变染色技法营造云雾缭绕的效果
                   3. **留白处理**：巧妙运用留白表现云雾和远山的虚实关系
                   4. **皴法模拟**：用扎染技法模拟传统绘画的皴法效果

                   **意境营造：**
                   追求"远山如黛，近水含烟"的诗意境界，体现中国传统美学的深远意境。

                   **文化内涵：**
                   体现了中国人对自然山水的深厚情感和哲学思考。
                   """,
            Summary = "层叠山峦与流动云雾的渐变扎染，模仿水墨皴法，营造深远空间感",
            Tags = ["山水", "传统", "皴法", "意境", "云雾"],
            Color = "#708090",
            ImageUrl = "/image/IMG_9017.JPG",
            ImageSmallUrl = "/image/IMG_9017.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-3),
            UpdatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        articles.Add(new Article
        {
            Title = "孤舟烟波",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   孤舟烟波是单只小舟点缀于朦胧水纹间，留白处如宣纸晕染，传递空寂禅意的诗意纹样。

                   **设计灵感：**
                   取材于"孤舟蓑笠翁，独钓寒江雪"的诗意境界，表现人与自然的和谐统一。

                   **艺术手法：**
                   1. **点景技法**：用小舟作为画面的点景元素，增强意境
                   2. **烟波表现**：通过特殊的扎染技法表现水面的烟波浩渺
                   3. **留白艺术**：大面积的留白营造空灵的意境
                   4. **晕染效果**：模拟宣纸的自然晕染效果

                   **禅意表达：**
                   体现了中国传统文化中的禅宗思想，追求内心的宁静与超脱。

                   **现代价值：**
                   在快节奏的现代生活中，这种纹样能够带给人们心灵的慰藉和精神的净化。
                   """,
            Summary = "单只小舟点缀于朦胧水纹间，留白处如宣纸晕染，传递空寂禅意",
            Tags = ["孤舟", "禅意", "传统", "诗意", "留白"],
            Color = "#B0C4DE",
            ImageUrl = "/image/IMG_9018.JPG",
            ImageSmallUrl = "/image/IMG_9018.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-4),
            UpdatedAt = DateTimeOffset.Now.AddDays(-4)
        });

        articles.Add(new Article
        {
            Title = "寒林远岫",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   寒林远岫是枯笔技法表现的冬日疏林，远山淡墨虚化，呈现萧疏冷逸的宋画格调。

                   **艺术风格：**
                   借鉴宋代山水画的枯笔技法，通过扎染工艺表现冬日山林的萧疏之美。

                   **技法特点：**
                   1. **枯笔模拟**：用特殊的扎结技法模拟国画中的枯笔效果
                   2. **疏密对比**：通过疏密变化表现林木的层次感
                   3. **远近虚实**：运用虚实对比表现远山近林的空间关系
                   4. **冷色调**：采用冷色调营造冬日的萧瑟氛围

                   **美学价值：**
                   体现了中国传统美学中"萧疏淡远"的审美理想，追求精神的超脱与升华。

                   **文化内涵：**
                   表现了文人雅士对自然的深刻理解和哲学思考。
                   """,
            Summary = "枯笔技法表现的冬日疏林，远山淡墨虚化，呈现萧疏冷逸的宋画格调",
            Tags = ["寒林", "传统", "宋画", "枯笔", "萧疏"],
            Color = "#696969",
            ImageUrl = "/image/IMG_9019.JPG",
            ImageSmallUrl = "/image/IMG_9019.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-5),
            UpdatedAt = DateTimeOffset.Now.AddDays(-5)
        });

        articles.Add(new Article
        {
            Title = "飞瀑生烟",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   飞瀑生烟是瀑布线条虚实相生，水雾部分采用防染技法，似宣纸吸水后的自然渗化效果。

                   **设计理念：**
                   表现瀑布飞流直下的壮观景象，以及水雾弥漫的朦胧美感。

                   **技法创新：**
                   1. **线条表现**：用流畅的线条表现瀑布的动态美
                   2. **防染技法**：采用防染技法表现水雾的朦胧效果
                   3. **虚实结合**：瀑布实写，水雾虚化，营造层次感
                   4. **自然渗化**：模拟宣纸的自然渗化效果

                   **动态美感：**
                   通过静态的扎染工艺表现动态的自然景象，体现了艺术的表现力。

                   **象征意义：**
                   瀑布象征着生命的活力和自然的力量，体现了人与自然的和谐关系。
                   """,
            Summary = "瀑布线条虚实相生，水雾部分采用防染技法，似宣纸吸水后的自然渗化",
            Tags = ["瀑布", "传统", "动态", "水雾", "自然"],
            Color = "#87CEEB",
            ImageUrl = "/image/IMG_9020.JPG",
            ImageSmallUrl = "/image/IMG_9020.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-6),
            UpdatedAt = DateTimeOffset.Now.AddDays(-6)
        });

        articles.Add(new Article
        {
            Title = "墨竹清风",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   墨竹清风是文人墨竹风韵，扎染表现墨色层次，展现君子品格与文化气息的传统纹样。

                   **文化背景：**
                   竹子在中国传统文化中象征着君子品格，"宁可食无肉，不可居无竹"体现了文人对竹的喜爱。

                   **艺术表现：**
                   1. **墨色层次**：通过不同深浅的染色表现墨竹的层次感
                   2. **竹节表现**：精细的扎结技法表现竹节的特征
                   3. **叶片飘逸**：表现竹叶在清风中的飘逸姿态
                   4. **意境营造**：追求"竹影横斜水清浅"的诗意境界

                   **品格象征：**
                   竹子的挺拔、坚韧、虚心等品质，象征着君子的高尚品格。

                   **现代价值：**
                   在现代社会中，墨竹纹样提醒人们保持内心的纯净和品格的高尚。
                   """,
            Summary = "文人墨竹风韵，扎染表现墨色层次，展现君子品格与文化气息",
            Tags = ["墨竹", "传统", "君子", "文人", "品格"],
            Color = "#2F4F4F",
            ImageUrl = "/image/IMG_9022.JPG",
            ImageSmallUrl = "/image/IMG_9022.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-7),
            UpdatedAt = DateTimeOffset.Now.AddDays(-7)
        });

        articles.Add(new Article
        {
            Title = "荷塘月色",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   荷塘月色是夏日荷塘的诗意描绘，月光洒向荷叶，营造宁静致远的东方美学意境。

                   **诗意来源：**
                   取材于朱自清的散文《荷塘月色》，表现夏夜荷塘的静谧美好。

                   **设计要素：**
                   1. **荷叶造型**：用圆形扎结表现荷叶的形态
                   2. **月光效果**：通过留白和渐变表现月光的洒落
                   3. **水波纹理**：细腻的水波纹表现荷塘的宁静
                   4. **色彩搭配**：淡雅的色彩营造夜色的朦胧美

                   **意境表达：**
                   追求"荷风送香气，竹露滴清响"的诗意境界，体现东方美学的含蓄与深远。

                   **文化价值：**
                   荷花在中国文化中象征着纯洁和高雅，"出淤泥而不染"的品格深受推崇。
                   """,
            Summary = "夏日荷塘的诗意描绘，月光洒向荷叶，营造宁静致远的东方美学",
            Tags = ["荷塘", "传统", "月色", "诗意", "东方美学"],
            Color = "#98FB98",
            ImageUrl = "/image/IMG_9023.JPG",
            ImageSmallUrl = "/image/IMG_9023.JPG",
            AuthorId = users.First(u => u.UserName == "sunchuanyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-8),
            UpdatedAt = DateTimeOffset.Now.AddDays(-8)
        });

        articles.Add(new Article
        {
            Title = "梅雪争艳",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   梅雪争艳是梅花与白雪的对比美学，展现坚韧不屈的精神品格，体现冬日的纯净与坚强。

                   **文化象征：**
                   梅花是中国传统文化中的"四君子"之一，象征着坚韧不屈、傲雪凌霜的精神品格。

                   **设计特色：**
                   1. **对比美学**：红梅与白雪的强烈对比，营造视觉冲击力
                   2. **枝干表现**：用流畅的线条表现梅花枝干的遒劲
                   3. **花朵刻画**：精细的扎结技法表现梅花的娇美
                   4. **雪花效果**：通过留白和点染表现雪花的纯净

                   **精神内涵：**
                   体现了中华民族不畏严寒、坚韧不屈的精神品格。

                   **艺术价值：**
                   将自然美与精神美完美结合，是传统文化的重要载体。
                   """,
            Summary = "梅花与白雪的对比美学，展现坚韧不屈的精神品格",
            Tags = ["梅花", "传统", "坚韧", "对比", "精神"],
            Color = "#DC143C",
            ImageUrl = "/image/IMG_9024.JPG",
            ImageSmallUrl = "/image/IMG_9024.JPG",
            AuthorId = users.First(u => u.UserName == "sunchuanyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-9),
            UpdatedAt = DateTimeOffset.Now.AddDays(-9)
        });

        articles.Add(new Article
        {
            Title = "春江花月",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   春江花月是"春江潮水连海平，海上明月共潮生"的诗意画面，展现春夜江景的美好意境。

                   **诗词来源：**
                   取材于张若虚的《春江花月夜》，被誉为"孤篇盖全唐"的千古名篇。

                   **意境营造：**
                   1. **江水表现**：用流动的线条表现春江的潮水
                   2. **月光洒落**：通过渐变技法表现月光在水面的反射
                   3. **花影点缀**：用点状扎结表现江边的花影
                   4. **整体和谐**：追求诗情画意的整体效果

                   **文化内涵：**
                   体现了中国古典诗词的深远意境和浪漫情怀。

                   **现代价值：**
                   在现代快节奏生活中，这种纹样能够带给人们诗意的慰藉。
                   """,
            Summary = "春江潮水连海平，海上明月共潮生的诗意画面",
            Tags = ["春江", "传统", "诗意", "月夜", "古典"],
            Color = "#4682B4",
            ImageUrl = "/image/IMG_9025.JPG",
            ImageSmallUrl = "/image/IMG_9025.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-10),
            UpdatedAt = DateTimeOffset.Now.AddDays(-10)
        });

        // 几何图案系列
        articles.Add(new Article
        {
            Title = "几何序列",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   几何序列是现代数学美学在扎染工艺中的体现，规律与变化的完美统一，展现数学之美。

                   **设计理念：**
                   将数学中的几何序列概念转化为视觉艺术，体现数学与艺术的完美结合。

                   **技法特点：**
                   1. **规律排列**：按照数学序列的规律进行图案排列
                   2. **渐变变化**：在规律中融入渐变变化，增加视觉层次
                   3. **精确控制**：运用精确的测量和计算确保图案的准确性
                   4. **现代工艺**：结合现代印染技术提高制作精度

                   **美学价值：**
                   体现了"数学是宇宙的语言"这一理念，将抽象的数学概念具象化。

                   **应用前景：**
                   适合现代建筑、工业设计等需要理性美感的领域。
                   """,
            Summary = "现代数学美学在扎染工艺中的体现，规律与变化的完美统一",
            Tags = ["几何", "数学", "序列", "规律", "现代"],
            Color = "#4169E1",
            ImageUrl = "/image/IMG_9026.JPG",
            ImageSmallUrl = "/image/IMG_9026.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-11),
            UpdatedAt = DateTimeOffset.Now.AddDays(-11)
        });

        articles.Add(new Article
        {
            Title = "抽象空间",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   抽象空间是突破传统束缚的现代抽象表达，色彩与形式的自由对话，展现艺术的无限可能。

                   **艺术理念：**
                   摆脱具象的束缚，通过抽象的形式语言表达内在的情感和思想。

                   **表现手法：**
                   1. **自由形态**：不拘泥于具体的物象，追求形式的自由表达
                   2. **色彩对话**：通过色彩的对比和呼应营造视觉张力
                   3. **空间构成**：运用现代构成原理组织画面空间
                   4. **情感表达**：通过抽象形式传达深层的情感内容

                   **创新价值：**
                   代表了传统扎染工艺向现代艺术的转型和发展。

                   **现代意义：**
                   体现了艺术家对传统工艺的现代化思考和创新实践。
                   """,
            Summary = "突破传统束缚的现代抽象表达，色彩与形式的自由对话",
            Tags = ["抽象", "现代", "自由", "色彩", "创新"],
            Color = "#FF6347",
            ImageUrl = "/image/IMG_9027.JPG",
            ImageSmallUrl = "/image/IMG_9027.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-12),
            UpdatedAt = DateTimeOffset.Now.AddDays(-12)
        });

        articles.Add(new Article
        {
            Title = "城市韵律",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   城市韵律是现代都市生活节奏在传统工艺中的诗意呈现，体现了传统与现代的时空对话。

                   **设计灵感：**
                   从现代都市的建筑线条、交通流线、霓虹灯光中汲取设计灵感。

                   **表现手法：**
                   1. **节奏感**：通过有规律的图案重复表现都市的节奏感
                   2. **线条运用**：用直线和曲线的组合表现城市的建筑美学
                   3. **色彩层次**：运用丰富的色彩层次表现都市的繁华
                   4. **现代工艺**：结合现代染色技术提升表现力

                   **文化意义：**
                   体现了传统工艺在现代社会中的适应性和生命力。

                   **时代价值：**
                   为传统扎染工艺注入了现代都市文化的新内涵。
                   """,
            Summary = "现代都市生活节奏在传统工艺中的诗意呈现",
            Tags = ["城市", "现代", "节奏", "都市", "时代"],
            Color = "#696969",
            ImageUrl = "/image/IMG_9028.JPG",
            ImageSmallUrl = "/image/IMG_9028.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-13),
            UpdatedAt = DateTimeOffset.Now.AddDays(-13)
        });

        // 民族风格系列
        articles.Add(new Article
        {
            Title = "彝族太阳",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   彝族太阳是彝族传统太阳图腾，象征光明与希望的民族文化符号，体现了彝族人民的太阳崇拜文化。

                   **文化背景：**
                   彝族是一个崇拜太阳的民族，太阳在彝族文化中象征着光明、温暖和希望。

                   **图腾特色：**
                   1. **放射状设计**：以太阳为中心的放射状图案设计
                   2. **神圣色彩**：使用红、黄等暖色调表现太阳的神圣
                   3. **几何化处理**：将太阳形象进行几何化的艺术处理
                   4. **民族特色**：融入彝族传统的装饰元素

                   **象征意义：**
                   太阳图腾象征着生命的活力、民族的团结和对美好生活的向往。

                   **传承价值：**
                   是彝族文化的重要载体，具有很高的民族文化价值。
                   """,
            Summary = "彝族传统太阳图腾，象征光明与希望的民族文化符号",
            Tags = ["彝族", "民族", "太阳", "图腾", "文化"],
            Color = "#FF4500",
            ImageUrl = "/image/IMG_9031.JPG",
            ImageSmallUrl = "/image/IMG_9031.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-14),
            UpdatedAt = DateTimeOffset.Now.AddDays(-14)
        });

        articles.Add(new Article
        {
            Title = "苗族银饰",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   苗族银饰是苗族传统银饰图案的扎染演绎，精致繁复的民族工艺美学，展现了苗族文化的精湛技艺。

                   **文化特色：**
                   苗族银饰是苗族文化的重要组成部分，具有深厚的历史文化底蕴。

                   **图案特点：**
                   1. **精致繁复**：图案精细复杂，层次丰富
                   2. **对称美学**：采用对称的构图方式，体现平衡美
                   3. **装饰性强**：具有很强的装饰效果和视觉冲击力
                   4. **工艺精湛**：体现了苗族工匠的精湛技艺

                   **文化内涵：**
                   银饰图案承载着苗族的历史记忆和文化传承。

                   **现代价值：**
                   为现代设计提供了丰富的民族文化元素。
                   """,
            Summary = "苗族传统银饰图案的扎染演绎，精致繁复的民族工艺美学",
            Tags = ["苗族", "民族", "银饰", "精致", "工艺"],
            Color = "#C0C0C0",
            ImageUrl = "/image/IMG_9032.JPG",
            ImageSmallUrl = "/image/IMG_9032.JPG",
            AuthorId = users.First(u => u.UserName == "sunchuanyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-15),
            UpdatedAt = DateTimeOffset.Now.AddDays(-15)
        });

        articles.Add(new Article
        {
            Title = "藏式祥云",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   藏式祥云是藏传佛教艺术中的祥云纹样，庄严神圣的宗教美学，体现了藏族文化的深厚底蕴。

                   **宗教背景：**
                   祥云在藏传佛教中具有神圣的宗教意义，象征着吉祥和护佑。

                   **艺术特色：**
                   1. **庄严神圣**：图案设计庄重大气，体现宗教的神圣性
                   2. **色彩丰富**：使用金、红、蓝等富有宗教色彩的颜色
                   3. **线条流畅**：云纹线条流畅优美，富有动感
                   4. **装饰华丽**：具有浓郁的装饰风格和民族特色

                   **文化价值：**
                   是藏族文化和藏传佛教艺术的重要载体。

                   **现代意义：**
                   为现代设计提供了独特的民族文化资源。
                   """,
            Summary = "藏传佛教艺术中的祥云纹样，庄严神圣的宗教美学",
            Tags = ["藏族", "民族", "祥云", "佛教", "神圣"],
            Color = "#8B0000",
            ImageUrl = "/image/IMG_9033.JPG",
            ImageSmallUrl = "/image/IMG_9033.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-16),
            UpdatedAt = DateTimeOffset.Now.AddDays(-16)
        });

        articles.Add(new Article
        {
            Title = "傣族孔雀",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   傣族孔雀是傣族文化中的孔雀图案，优雅华丽的民族特色，体现了傣族人民对美好生活的向往。

                   **文化背景：**
                   孔雀在傣族文化中是吉祥的象征，代表着美丽、高贵和幸福。

                   **设计特色：**
                   1. **优雅造型**：孔雀的优美身姿和华丽羽毛
                   2. **色彩绚丽**：使用蓝、绿、金等鲜艳色彩
                   3. **装饰华美**：羽毛的精细刻画和装饰效果
                   4. **动态美感**：表现孔雀开屏的动态美

                   **象征意义：**
                   孔雀象征着吉祥如意、富贵荣华和美好未来。

                   **艺术价值：**
                   是傣族传统文化艺术的重要组成部分。
                   """,
            Summary = "傣族文化中的孔雀图案，优雅华丽的民族特色",
            Tags = ["傣族", "民族", "孔雀", "优雅", "华丽"],
            Color = "#00CED1",
            ImageUrl = "/image/IMG_9034.JPG",
            ImageSmallUrl = "/image/IMG_9034.JPG",
            AuthorId = users.First(u => u.UserName == "sunchuanyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-17),
            UpdatedAt = DateTimeOffset.Now.AddDays(-17)
        });

        // 更多几何图案
        articles.Add(new Article
        {
            Title = "回族几何",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   回族几何是回族传统几何图案，简洁优美的伊斯兰艺术特色，体现了几何美学的精髓。

                   **艺术特色：**
                   伊斯兰艺术以几何图案为主要特征，追求数学的精确性和美学的和谐性。

                   **设计原理：**
                   1. **几何精确**：严格按照几何原理进行设计
                   2. **对称美学**：采用多种对称形式营造平衡美
                   3. **无限延展**：图案可以无限重复延展
                   4. **简洁优美**：线条简洁，形式优美

                   **文化内涵：**
                   体现了伊斯兰文化对秩序和和谐的追求。

                   **现代价值：**
                   为现代几何设计提供了重要的文化资源。
                   """,
            Summary = "回族传统几何图案，简洁优美的伊斯兰艺术特色",
            Tags = ["回族", "几何", "伊斯兰", "简洁", "对称"],
            Color = "#4682B4",
            ImageUrl = "/image/IMG_9040.JPG",
            ImageSmallUrl = "/image/IMG_9040.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-18),
            UpdatedAt = DateTimeOffset.Now.AddDays(-18)
        });

        articles.Add(new Article
        {
            Title = "圆形变奏",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   圆形变奏是以圆为基础的几何变化，展现数学之美，体现了几何图形的无限可能性。

                   **设计理念：**
                   以圆这一最基本的几何形状为基础，通过各种变化创造丰富的视觉效果。

                   **变化手法：**
                   1. **大小变化**：不同大小的圆形组合
                   2. **位置变化**：圆形的不同排列方式
                   3. **重叠效果**：圆形之间的重叠关系
                   4. **色彩变化**：通过色彩变化增强视觉效果

                   **数学美学：**
                   体现了数学中的比例、对称、节奏等美学原理。

                   **应用价值：**
                   在现代设计中有广泛的应用前景。
                   """,
            Summary = "以圆为基础的几何变化，展现数学之美",
            Tags = ["圆形", "几何", "变奏", "数学", "美学"],
            Color = "#FF69B4",
            ImageUrl = "/image/IMG_9041.JPG",
            ImageSmallUrl = "/image/IMG_9041.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-19),
            UpdatedAt = DateTimeOffset.Now.AddDays(-19)
        });

        articles.Add(new Article
        {
            Title = "三角韵律",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   三角韵律是三角形的重复与变化，构成富有节奏感的几何图案，展现几何形态的动感美。

                   **设计特点：**
                   三角形作为最稳定的几何形状，通过重复和变化可以创造出丰富的韵律感。

                   **构成方法：**
                   1. **重复排列**：三角形的有规律重复
                   2. **大小变化**：不同大小三角形的组合
                   3. **方向变化**：三角形的不同朝向
                   4. **色彩韵律**：通过色彩变化增强韵律感

                   **视觉效果：**
                   创造出强烈的节奏感和动感，具有很强的视觉冲击力。

                   **现代应用：**
                   在建筑、工业设计等领域有重要应用价值。
                   """,
            Summary = "三角形的重复与变化，构成富有节奏感的几何图案",
            Tags = ["三角", "几何", "韵律", "节奏", "动感"],
            Color = "#32CD32",
            ImageUrl = "/image/IMG_9042.JPG",
            ImageSmallUrl = "/image/IMG_9042.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-20),
            UpdatedAt = DateTimeOffset.Now.AddDays(-20)
        });

        articles.Add(new Article
        {
            Title = "方格世界",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   方格世界是方格的排列组合，简单中蕴含无限可能，体现了几何美学的基础原理。

                   **设计理念：**
                   方格作为最基本的几何单元，通过不同的排列组合可以创造出无限的可能性。

                   **构成原理：**
                   1. **基础单元**：以正方形为基本构成单元
                   2. **排列变化**：不同的排列方式创造不同效果
                   3. **色彩搭配**：通过色彩变化增强视觉层次
                   4. **比例关系**：不同大小方格的比例关系

                   **美学价值：**
                   体现了"简单即美"的设计理念，在简洁中蕴含丰富。

                   **应用前景：**
                   在现代建筑、室内设计、平面设计等领域有广泛应用。
                   """,
            Summary = "方格的排列组合，简单中蕴含无限可能",
            Tags = ["方格", "几何", "排列", "简单", "可能"],
            Color = "#708090",
            ImageUrl = "/image/IMG_9043.JPG",
            ImageSmallUrl = "/image/IMG_9043.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-21),
            UpdatedAt = DateTimeOffset.Now.AddDays(-21)
        });

        articles.Add(new Article
        {
            Title = "螺旋宇宙",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   螺旋宇宙是螺旋线条的延展，象征宇宙的无限循环，体现了自然界的基本运动规律。

                   **设计灵感：**
                   从宇宙星系的螺旋结构、DNA的双螺旋结构等自然现象中汲取灵感。

                   **表现手法：**
                   1. **螺旋线条**：流畅的螺旋线条表现动态美
                   2. **渐变效果**：从中心向外的渐变变化
                   3. **无限延展**：螺旋的无限延展特性
                   4. **宇宙色彩**：使用深蓝、紫色等宇宙色彩

                   **哲学内涵：**
                   螺旋象征着生命的循环、时间的流逝和宇宙的运行规律。

                   **现代意义：**
                   体现了人类对宇宙奥秘的探索和思考。
                   """,
            Summary = "螺旋线条的延展，象征宇宙的无限循环",
            Tags = ["螺旋", "几何", "宇宙", "循环", "无限"],
            Color = "#4B0082",
            ImageUrl = "/image/IMG_9044.JPG",
            ImageSmallUrl = "/image/IMG_9044.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-22),
            UpdatedAt = DateTimeOffset.Now.AddDays(-22)
        });

        articles.Add(new Article
        {
            Title = "对称美学",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   对称美学是对称图案的经典美学，平衡与和谐的视觉享受，体现了美学的基本原理。

                   **美学原理：**
                   对称是美学中的基本原理之一，能够给人以平衡、和谐、稳定的美感体验。

                   **对称类型：**
                   1. **轴对称**：以轴线为中心的对称
                   2. **中心对称**：以点为中心的对称
                   3. **旋转对称**：通过旋转形成的对称
                   4. **平移对称**：通过平移形成的对称

                   **视觉效果：**
                   创造出平衡、和谐、稳定的视觉效果，给人以美的享受。

                   **文化价值：**
                   对称美学在各种文化中都有重要地位，是人类共同的美学追求。
                   """,
            Summary = "对称图案的经典美学，平衡与和谐的视觉享受",
            Tags = ["对称", "几何", "美学", "平衡", "和谐"],
            Color = "#DAA520",
            ImageUrl = "/image/IMG_9045.JPG",
            ImageSmallUrl = "/image/IMG_9045.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-23),
            UpdatedAt = DateTimeOffset.Now.AddDays(-23)
        });

        // 现代风格系列
        articles.Add(new Article
        {
            Title = "渐变幻彩",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   渐变幻彩是色彩渐变的现代技法，如彩虹般的梦幻效果，展现了现代染色技术的魅力。

                   **技术特色：**
                   运用现代染色技术实现平滑的色彩渐变效果，创造出梦幻般的视觉体验。

                   **制作工艺：**
                   1. **渐变技法**：精确控制染料的渐变过程
                   2. **色彩搭配**：科学的色彩搭配理论
                   3. **技术控制**：现代化的技术控制手段
                   4. **效果呈现**：追求完美的渐变效果

                   **视觉效果：**
                   创造出如彩虹般绚丽的色彩效果，具有强烈的视觉冲击力。

                   **现代价值：**
                   代表了传统扎染工艺与现代技术的完美结合。
                   """,
            Summary = "色彩渐变的现代技法，如彩虹般的梦幻效果",
            Tags = ["渐变", "现代", "色彩", "梦幻", "技法"],
            Color = "#FF1493",
            ImageUrl = "/image/IMG_9051.JPG",
            ImageSmallUrl = "/image/IMG_9051.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-24),
            UpdatedAt = DateTimeOffset.Now.AddDays(-24)
        });

        articles.Add(new Article
        {
            Title = "立体空间",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   立体空间是三维空间感的二维表达，现代视觉艺术的探索，体现了艺术表现的创新突破。

                   **设计理念：**
                   在二维平面上创造三维空间的视觉效果，挑战传统平面艺术的表现极限。

                   **表现技法：**
                   1. **透视效果**：运用透视原理创造空间感
                   2. **明暗对比**：通过明暗变化表现立体感
                   3. **层次关系**：营造前后层次的空间关系
                   4. **视觉错觉**：利用视觉错觉增强立体效果

                   **艺术价值：**
                   代表了现代艺术对传统表现手法的突破和创新。

                   **现代意义：**
                   为传统扎染工艺注入了现代艺术的新理念。
                   """,
            Summary = "三维空间感的二维表达，现代视觉艺术的探索",
            Tags = ["立体", "现代", "空间", "视觉", "艺术"],
            Color = "#9370DB",
            ImageUrl = "/image/IMG_9052.JPG",
            ImageSmallUrl = "/image/IMG_9052.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-25),
            UpdatedAt = DateTimeOffset.Now.AddDays(-25)
        });

        articles.Add(new Article
        {
            Title = "解构重组",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   解构重组是传统元素的解构与重组，后现代艺术的实验，体现了艺术创作的新思维。

                   **创作理念：**
                   将传统的纹样元素进行解构，然后以全新的方式重新组合，创造出具有现代感的新作品。

                   **创作手法：**
                   1. **元素分解**：将传统纹样分解为基本元素
                   2. **重新组合**：以新的逻辑重新组合这些元素
                   3. **形式创新**：在形式上进行大胆创新
                   4. **意义重构**：赋予传统元素新的文化意义

                   **艺术特色：**
                   具有强烈的实验性和前卫性，体现了后现代艺术的特征。

                   **文化价值：**
                   为传统文化的现代传承提供了新的思路。
                   """,
            Summary = "传统元素的解构与重组，后现代艺术的实验",
            Tags = ["解构", "现代", "重组", "实验", "创新"],
            Color = "#FF4500",
            ImageUrl = "/image/IMG_9053.JPG",
            ImageSmallUrl = "/image/IMG_9053.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-26),
            UpdatedAt = DateTimeOffset.Now.AddDays(-26)
        });

        articles.Add(new Article
        {
            Title = "流体艺术",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   流体艺术是流体形态的艺术表达，自然与科技的结合，展现了现代艺术的新可能性。

                   **设计灵感：**
                   从流体力学、自然界的流体现象中汲取灵感，创造出具有流动感的艺术作品。

                   **表现特色：**
                   1. **流动感**：表现流体的动态美感
                   2. **自然形态**：模拟自然界的流体现象
                   3. **科技结合**：运用现代科技手段
                   4. **抽象表达**：通过抽象形式表达流体美学

                   **技术创新：**
                   结合现代流体力学原理和艺术表现手法。

                   **现代价值：**
                   代表了艺术与科学的跨界融合。
                   """,
            Summary = "流体形态的艺术表达，自然与科技的结合",
            Tags = ["流体", "现代", "科技", "自然", "动态"],
            Color = "#00BFFF",
            ImageUrl = "/image/IMG_9054.JPG",
            ImageSmallUrl = "/image/IMG_9054.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-27),
            UpdatedAt = DateTimeOffset.Now.AddDays(-27)
        });

        articles.Add(new Article
        {
            Title = "像素世界",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   像素世界是像素化的艺术表达，数字时代的美学符号，体现了数字文化对传统工艺的影响。

                   **时代背景：**
                   在数字化时代，像素成为了重要的视觉元素和文化符号。

                   **设计特色：**
                   1. **像素化处理**：将传统图案进行像素化处理
                   2. **数字美学**：体现数字时代的美学特征
                   3. **复古未来**：结合复古游戏和未来科技的美学
                   4. **文化符号**：成为数字文化的重要符号

                   **文化意义：**
                   代表了传统工艺在数字时代的新发展方向。

                   **应用价值：**
                   在游戏、动漫、数字媒体等领域有重要应用。
                   """,
            Summary = "像素化的艺术表达，数字时代的美学符号",
            Tags = ["像素", "现代", "数字", "游戏", "文化"],
            Color = "#32CD32",
            ImageUrl = "/image/IMG_9055.JPG",
            ImageSmallUrl = "/image/IMG_9055.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-28),
            UpdatedAt = DateTimeOffset.Now.AddDays(-28)
        });

        articles.Add(new Article
        {
            Title = "全息投影",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   全息投影是全息效果的视觉模拟，未来科技的艺术呈现，展现了科技与艺术结合的无限可能。

                   **科技背景：**
                   全息技术代表了未来显示技术的发展方向，具有强烈的科幻色彩。

                   **视觉效果：**
                   1. **立体感**：营造强烈的三维立体视觉效果
                   2. **光影变化**：模拟全息投影的光影变化
                   3. **未来感**：体现未来科技的美学特征
                   4. **互动性**：暗示人机交互的可能性

                   **艺术价值：**
                   代表了艺术对未来科技的想象和表达。

                   **文化意义：**
                   体现了人类对未来科技发展的憧憬和思考。
                   """,
            Summary = "全息效果的视觉模拟，未来科技的艺术呈现",
            Tags = ["全息", "现代", "科技", "未来", "投影"],
            Color = "#00FFFF",
            ImageUrl = "/image/IMG_9056.JPG",
            ImageSmallUrl = "/image/IMG_9056.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-29),
            UpdatedAt = DateTimeOffset.Now.AddDays(-29)
        });

        articles.Add(new Article
        {
            Title = "分形几何",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   分形几何是分形几何的自相似美学，数学与艺术的完美结合，展现了自然界的深层规律。

                   **数学原理：**
                   分形几何是现代数学的重要分支，揭示了自然界中的自相似规律。

                   **艺术表现：**
                   1. **自相似性**：在不同尺度上呈现相似的结构
                   2. **无限细节**：可以无限放大而不失去细节
                   3. **自然美学**：体现自然界的内在美学规律
                   4. **复杂性**：从简单规则产生复杂图案

                   **科学价值：**
                   体现了数学、科学与艺术的深度融合。

                   **现代意义：**
                   为现代设计提供了全新的理论基础和表现手法。
                   """,
            Summary = "分形几何的自相似美学，数学与艺术的完美结合",
            Tags = ["分形", "现代", "几何", "数学", "自然"],
            Color = "#8A2BE2",
            ImageUrl = "/image/IMG_9057.JPG",
            ImageSmallUrl = "/image/IMG_9057.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-30),
            UpdatedAt = DateTimeOffset.Now.AddDays(-30)
        });

        articles.Add(new Article
        {
            Title = "量子纠缠",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   量子纠缠是量子物理概念的艺术化表达，科学与美学的对话，体现了前沿科学的艺术魅力。

                   **科学背景：**
                   量子纠缠是量子物理学中的重要现象，具有神秘而深刻的科学内涵。

                   **艺术诠释：**
                   1. **神秘感**：表现量子世界的神秘特性
                   2. **关联性**：体现量子纠缠的关联特性
                   3. **抽象美**：通过抽象形式表达科学概念
                   4. **未来感**：展现前沿科学的未来感

                   **文化价值：**
                   代表了艺术对前沿科学的理解和表达。

                   **时代意义：**
                   体现了科学时代艺术创作的新方向。
                   """,
            Summary = "量子物理概念的艺术化表达，科学与美学的对话",
            Tags = ["量子", "现代", "科学", "物理", "神秘"],
            Color = "#4B0082",
            ImageUrl = "/image/IMG_9058.JPG",
            ImageSmallUrl = "/image/IMG_9058.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-31),
            UpdatedAt = DateTimeOffset.Now.AddDays(-31)
        });

        articles.Add(new Article
        {
            Title = "生物形态",
            Type = ArticleType.Gene,
            Status = ArticleStatus.Published,
            Body = """
                   生物形态是生物结构的仿生设计，自然形态的现代诠释，展现了自然界的设计智慧。

                   **仿生理念：**
                   从自然界的生物结构中汲取设计灵感，将生物的优美形态转化为艺术表达。

                   **设计特色：**
                   1. **有机形态**：模拟生物的有机形态特征
                   2. **功能美学**：体现生物结构的功能性美学
                   3. **自然智慧**：展现自然界的设计智慧
                   4. **现代诠释**：用现代手法诠释自然形态

                   **科学价值：**
                   体现了仿生学在艺术设计中的应用。

                   **现代意义：**
                   为可持续设计提供了重要的理论支撑。
                   """,
            Summary = "生物结构的仿生设计，自然形态的现代诠释",
            Tags = ["生物", "现代", "仿生", "自然", "形态"],
            Color = "#228B22",
            ImageUrl = "/image/IMG_9059.JPG",
            ImageSmallUrl = "/image/IMG_9059.JPG",
            AuthorId = users.First(u => u.UserName == "chenranmo").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-32),
            UpdatedAt = DateTimeOffset.Now.AddDays(-32)
        });

        // 创意工坊文章
        articles.Add(new Article
        {
            Title = "扎染DIY入门教程",
            Type = ArticleType.Post,
            Status = ArticleStatus.Published,
            Body = """
                   想要学习扎染？这里有最详细的入门教程，从材料准备到技法掌握，让你轻松入门扎染艺术。

                   **材料准备：**
                   1. **布料选择**：纯棉或亚麻布料，白色或浅色为佳
                   2. **染料准备**：天然靛蓝染料或化学染料
                   3. **扎结工具**：棉绳、橡皮筋、夹子等
                   4. **辅助工具**：手套、围裙、搅拌棒、容器

                   **基础技法：**

                   **圆形纹样制作：**
                   1. 将布料平铺，选择中心点
                   2. 用手指捏起中心点，形成锥形
                   3. 用绳子从顶端开始螺旋式绑扎
                   4. 绑扎要紧密均匀，形成防染效果

                   **条纹纹样制作：**
                   1. 将布料折叠成手风琴状
                   2. 用绳子在折叠处绑扎
                   3. 绑扎间距决定条纹宽度
                   4. 可以调整绑扎松紧度控制染色效果

                   **染色步骤：**
                   1. **预处理**：布料先用清水浸湿
                   2. **调制染液**：按比例调制染料溶液
                   3. **浸染过程**：将扎好的布料浸入染液
                   4. **时间控制**：根据需要的颜色深度控制浸染时间
                   5. **清洗定色**：用清水冲洗至无浮色

                   **注意事项：**
                   - 操作时佩戴手套，避免染料接触皮肤
                   - 染色环境要通风良好
                   - 不同颜色的染料要分开使用
                   - 作品完成后要充分晾干

                   通过这个入门教程，你就可以制作出属于自己的扎染作品了！
                   """,
            Summary = "扎染DIY的完整入门指南，包含材料准备、基础技法和染色步骤",
            Tags = ["DIY", "教程", "入门", "扎染", "手工", "创意工坊"],
            Color = "#3A7BD5",
            ImageUrl = "/image/IMG_9067.JPG",
            ImageSmallUrl = "/image/IMG_9067.JPG",
            AuthorId = users.First(u => u.UserName == "qianchuangyi").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-3),
            UpdatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        // 高级技法教程
        articles.Add(new Article
        {
            Title = "千叠冰纹染高级技法",
            Type = ArticleType.Post,
            Status = ArticleStatus.Published,
            Body = """
                   千叠冰纹染是扎染技艺中的最高境界，需要深厚的功底和精湛的技艺才能掌握。

                   **技法特点：**
                   千叠冰纹染得名于其独特的纹样效果——如千层冰花般晶莹剔透，层次分明。这种技法要求极高的精确度和丰富的经验。

                   **制作要点：**
                   1. **布料选择**：必须使用高品质的真丝或细棉布
                   2. **扎结技巧**：采用特殊的'螺旋叠扎'法，需要精确控制每一层的松紧度
                   3. **染料配制**：使用传统的天然靛蓝，需要经过特殊的发酵处理
                   4. **浸染控制**：分多次浸染，每次间隔时间和深度都有严格要求

                   **传承意义：**
                   千叠冰纹染不仅是一种技法，更是传统文化的载体。掌握这一技法的工匠被称为'染师'，是扎染界的最高荣誉。

                   **学习建议：**
                   - 需要先掌握基础扎染技法
                   - 建议在有经验的师傅指导下学习
                   - 需要大量的练习和实践
                   - 要有耐心和毅力，不可急于求成

                   这一技法的传承关系到扎染文化的延续，希望更多的人能够学习和掌握。
                   """,
            Summary = "扎染技艺中的最高境界——千叠冰纹染的技法要点和传承意义",
            Tags = ["千叠冰纹染", "高级技法", "传承", "扎染", "教程"],
            Color = "#182848",
            ImageUrl = "/image/IMG_9053.JPG",
            ImageSmallUrl = "/image/IMG_9053.JPG",
            AuthorId = users.First(u => u.UserName == "zhaoshewen").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-2),
            UpdatedAt = DateTimeOffset.Now.AddDays(-2)
        });

        // 现代应用案例
        articles.Add(new Article
        {
            Title = "扎染在现代时装设计中的应用",
            Type = ArticleType.Post,
            Status = ArticleStatus.Published,
            Body = """
                   扎染技艺在现代时装设计中焕发出新的生命力，成为设计师们青睐的创意元素。

                   **时装应用趋势：**
                   1. **高端定制**：奢侈品牌将扎染元素融入高端定制服装
                   2. **街头时尚**：扎染图案成为街头潮流的重要元素
                   3. **可持续时尚**：天然染料符合环保理念，受到推崇
                   4. **文化融合**：东西方设计理念的完美结合

                   **设计创新点：**
                   - 将传统纹样与现代剪裁结合
                   - 运用新型面料提升扎染效果
                   - 结合数字化设计预览效果
                   - 创新染色技术提高色彩稳定性

                   **成功案例：**
                   - 国际时装周上的扎染主题系列
                   - 知名品牌的扎染联名款
                   - 独立设计师的创新作品
                   - 传统工艺与现代科技的结合

                   **市场前景：**
                   随着人们对传统文化的重新认识和对个性化产品的需求增长，扎染在时装领域的应用前景广阔。

                   扎染不仅是传统工艺的传承，更是现代创意的源泉。
                   """,
            Summary = "扎染技艺在现代时装设计中的创新应用和发展趋势",
            Tags = ["扎染", "时装设计", "现代应用", "创新", "时尚"],
            Color = "#E85A4F",
            ImageUrl = "/image/IMG_9063.JPG",
            ImageSmallUrl = "/image/IMG_9063.JPG",
            AuthorId = users.First(u => u.UserName == "liwenchuang").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        dbContext.Articles.AddRange(articles);
        await dbContext.SaveChangesAsync();
        return articles;
    }

    private async Task<List<Good>> CreateGoods(List<User> users, List<Article> articles)
    {
        var goods = new List<Good>();

        // 服饰类
        // 扎染围巾 - 基于shop.html中的"蓝染云纹手工围巾"
        goods.Add(new Good
        {
            Name = "蓝染云纹手工围巾 - 传统扎染工艺",
            Price = 398.0,
            DiscountedPrice = 298.0,
            Stoke = 50,
            Status = GoodStatus.Available,
            PublisherName = "张染艺工作室",
            ImageUrl = "/image/IMG_9034.JPG",
            PublisherId = users.First(u => u.UserName == "zhaoshewen").Id,
            RelatedArticleId = articles.First(a => a.Title == "云水纹的设计原理").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-7),
            UpdatedAt = DateTimeOffset.Now.AddDays(-7)
        });

        // 现代几何T恤 - 基于shop.html中的产品
        goods.Add(new Good
        {
            Name = "现代几何扎染纯棉T恤",
            Price = 168.0,
            DiscountedPrice = 168.0,
            Stoke = 100,
            Status = GoodStatus.Available,
            PublisherName = "李纹创设计",
            ImageUrl = "/image/IMG_9063.JPG",
            PublisherId = users.First(u => u.UserName == "liwenchuang").Id,
            RelatedArticleId = articles.First(a => a.Title == "几何拼接纹样创新设计").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-6),
            UpdatedAt = DateTimeOffset.Now.AddDays(-6)
        });

        // 家居用品类
        // 传统纹样靠垫套 - 基于shop.html中的产品
        goods.Add(new Good
        {
            Name = "传统纹样扎染靠垫套(一对装)",
            Price = 128.0,
            DiscountedPrice = 128.0,
            Stoke = 30,
            Status = GoodStatus.Available,
            PublisherName = "王艺坊",
            ImageUrl = "/image/IMG_9064.JPG",
            PublisherId = users.First(u => u.UserName == "wangyifang").Id,
            RelatedArticleId = articles.First(a => a.Title == "传统花卉纹样的现代演绎").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-5),
            UpdatedAt = DateTimeOffset.Now.AddDays(-5)
        });

        // 艺术品类
        // 山水意境装饰画 - 基于shop.html中的产品
        goods.Add(new Good
        {
            Name = "山水意境扎染装饰画(40×60cm)",
            Price = 580.0,
            DiscountedPrice = 580.0,
            Stoke = 15,
            Status = GoodStatus.Available,
            PublisherName = "陈染墨艺术工作室",
            ImageUrl = "/image/IMG_9065.JPG",
            PublisherId = users.First(u => u.UserName == "chenranmo").Id,
            RelatedArticleId = articles.First(a => a.Title == "云水纹的设计原理").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-4),
            UpdatedAt = DateTimeOffset.Now.AddDays(-4)
        });

        // 家居用品类
        // 民族风餐桌布 - 基于shop.html中的产品
        goods.Add(new Good
        {
            Name = "民族风扎染餐桌布(140×200cm)",
            Price = 320.0,
            DiscountedPrice = 320.0,
            Stoke = 25,
            Status = GoodStatus.Available,
            PublisherName = "赵设纹工作室",
            ImageUrl = "/image/IMG_9066.JPG",
            PublisherId = users.First(u => u.UserName == "zhaoshewen").Id,
            RelatedArticleId = articles.First(a => a.Title == "传统花卉纹样的现代演绎").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-3),
            UpdatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        // 原材料类
        // DIY材料包 - 基于shop.html中的产品
        goods.Add(new Good
        {
            Name = "扎染DIY入门材料包(含教程)",
            Price = 98.0,
            DiscountedPrice = 98.0,
            Stoke = 200,
            Status = GoodStatus.Available,
            PublisherName = "钱创艺工作室",
            ImageUrl = "/image/IMG_9067.JPG",
            PublisherId = users.First(u => u.UserName == "qianchuangyi").Id,
            RelatedArticleId = articles.First(a => a.Title == "扎染DIY入门教程").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-2),
            UpdatedAt = DateTimeOffset.Now.AddDays(-2)
        });

        // 版权交易类 - 基于shop.html中的版权交易区
        // 版权商品 - 云水纹设计版权
        goods.Add(new Good
        {
            Name = "云水纹设计版权(商用授权)",
            Price = 1200.0,
            DiscountedPrice = 1200.0,
            Stoke = 1,
            Status = GoodStatus.Available,
            PublisherName = "赵设纹工作室",
            ImageUrl = "/image/IMG_9056.JPG",
            PublisherId = users.First(u => u.UserName == "zhaoshewen").Id,
            RelatedArticleId = articles.First(a => a.Title == "云水纹的设计原理").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        // 版权商品 - 几何拼接纹样
        goods.Add(new Good
        {
            Name = "几何拼接纹样(个人授权)",
            Price = 980.0,
            DiscountedPrice = 980.0,
            Stoke = 1,
            Status = GoodStatus.Available,
            PublisherName = "钱创艺工作室",
            ImageUrl = "/image/IMG_9057.JPG",
            PublisherId = users.First(u => u.UserName == "qianchuangyi").Id,
            RelatedArticleId = articles.First(a => a.Title == "几何拼接纹样创新设计").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        // 版权商品 - 传统花卉纹样
        goods.Add(new Good
        {
            Name = "传统花卉纹样(商用授权)",
            Price = 1500.0,
            DiscountedPrice = 1500.0,
            Stoke = 1,
            Status = GoodStatus.Available,
            PublisherName = "孙传艺工作室",
            ImageUrl = "/image/IMG_9058.JPG",
            PublisherId = users.First(u => u.UserName == "sunchuanyi").Id,
            RelatedArticleId = articles.First(a => a.Title == "传统花卉纹样的现代演绎").Id,
            CreatedAt = DateTimeOffset.Now,
            UpdatedAt = DateTimeOffset.Now
        });

        // 更多服饰类产品
        goods.Add(new Good
        {
            Name = "手工扎染亚麻连衣裙",
            Price = 458.0,
            DiscountedPrice = 398.0,
            Stoke = 20,
            Status = GoodStatus.Available,
            PublisherName = "李纹创设计",
            ImageUrl = "/image/IMG_9060.JPG",
            PublisherId = users.First(u => u.UserName == "liwenchuang").Id,
            RelatedArticleId = articles.First(a => a.Title == "扎染在现代时装设计中的应用").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-3),
            UpdatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        goods.Add(new Good
        {
            Name = "传统蓝染棉麻披肩",
            Price = 228.0,
            DiscountedPrice = 198.0,
            Stoke = 35,
            Status = GoodStatus.Available,
            PublisherName = "孙传艺工作室",
            ImageUrl = "/image/IMG_9061.JPG",
            PublisherId = users.First(u => u.UserName == "sunchuanyi").Id,
            RelatedArticleId = articles.First(a => a.Title == "传统花卉纹样的现代演绎").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-4),
            UpdatedAt = DateTimeOffset.Now.AddDays(-4)
        });

        // 更多家居用品
        goods.Add(new Good
        {
            Name = "扎染工艺茶席布套装",
            Price = 188.0,
            DiscountedPrice = 168.0,
            Stoke = 40,
            Status = GoodStatus.Available,
            PublisherName = "王艺坊",
            ImageUrl = "/image/IMG_9059.JPG",
            PublisherId = users.First(u => u.UserName == "wangyifang").Id,
            RelatedArticleId = articles.First(a => a.Title == "扎染工艺的历史与发展").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-5),
            UpdatedAt = DateTimeOffset.Now.AddDays(-5)
        });

        goods.Add(new Good
        {
            Name = "手工扎染窗帘(定制款)",
            Price = 680.0,
            DiscountedPrice = 580.0,
            Stoke = 10,
            Status = GoodStatus.Available,
            PublisherName = "陈染墨艺术工作室",
            ImageUrl = "/image/IMG_9055.JPG",
            PublisherId = users.First(u => u.UserName == "chenranmo").Id,
            RelatedArticleId = articles.First(a => a.Title == "云水纹的设计原理").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-6),
            UpdatedAt = DateTimeOffset.Now.AddDays(-6)
        });

        // 高级材料包
        goods.Add(new Good
        {
            Name = "专业扎染工具套装(含天然染料)",
            Price = 298.0,
            DiscountedPrice = 268.0,
            Stoke = 50,
            Status = GoodStatus.Available,
            PublisherName = "钱创艺工作室",
            ImageUrl = "/image/IMG_9054.JPG",
            PublisherId = users.First(u => u.UserName == "qianchuangyi").Id,
            RelatedArticleId = articles.First(a => a.Title == "千叠冰纹染高级技法").Id,
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        dbContext.Goods.AddRange(goods);
        await dbContext.SaveChangesAsync();
        return goods;
    }

    private async Task<List<Collection>> CreateArticleCollections(List<User> users, List<Article> articles)
    {
        var collections = new List<Collection>();
        var admin = users.First(u => u.UserName == "admin");

        // IP故事集合 - 对应 ip-story.html 的需求
        var storyArticles = articles.Where(a => a.Type == ArticleType.Story).ToList();
        var characterArticles = articles.Where(a => a.Type == ArticleType.Character).ToList();

        collections.Add(new Collection
        {
            Id = Guid.CreateVersion7(),
            Name = "扎染艺术",
            Summary = "探索千年扎染技艺背后的传奇人物与守护灵，感受传统工艺与现代困境的碰撞。",
            Description = """
                          ## 背景故事

                          ### 千年染灵起源
                          唐代盐商云集的自贡，染娘苏氏创"八技封灵术"—将毕生执念注入代表不同技法的纹样，化作守护染艺的灵体。唯有苏氏血脉能唤醒染灵，但每代传承者需以心血供养灵体，否则将遭染疾反噬。

                          ### 青蓝坊的现代困境
                          - **空间侵占：** 祖传靛蓝灵泉被开发商填埋，建度假村
                          - **技艺断代：** 母亲去世，盐夏对"干叠冰纹染"心有排斥
                          - **染灵危机：** 近十年无人施展完整古法，染灵陷入濒死沉睡
                          """,
            Color = "#3A7BD5",
            Tags = ["扎染", "传统工艺", "IP故事", "文化传承"],
            Type = CollectionType.Story,
            ChildrenIds = storyArticles.Concat(characterArticles).Select(a => a.Id).ToArray(),
            CreatorId = admin.Id,
            Creator = admin
        });

        // 扎染工艺百科集合 - 对应 knowledge.html 的需求
        var wikiArticles = articles.Where(a => a.Type == ArticleType.Wiki).ToList();

        collections.Add(new Collection
        {
            Id = Guid.CreateVersion7(),
            Name = "扎染工艺百科",
            Summary = "扎染技艺的完整知识体系",
            Description = "从历史起源到现代发展，从基础技法到高级工艺，全面介绍扎染技艺的知识体系。适合初学者了解和专业人士参考。",
            Color = "#00A896",
            Tags = ["百科", "工艺", "技法", "历史", "知识"],
            Type = CollectionType.Article,
            ChildrenIds = wikiArticles.Select(a => a.Id).ToArray(),
            CreatorId = admin.Id,
            Creator = admin
        });

        // 纹样基因库集合 - 对应 pattern-library.html 的需求
        var geneArticles = articles.Where(a => a.Type == ArticleType.Gene).ToList();

        collections.Add(new Collection
        {
            Id = Guid.CreateVersion7(),
            Name = "纹样基因库",
            Summary = "传统与现代纹样设计的创新融合",
            Description = "收录各种扎染纹样的设计原理、制作技法和创新应用。从传统云水纹到现代几何图案，为设计师和爱好者提供丰富的创作灵感。",
            Color = "#F7B733",
            Tags = ["纹样", "设计", "基因库", "创新", "传统"],
            Type = CollectionType.Article,
            ChildrenIds = geneArticles.Select(a => a.Id).ToArray(),
            CreatorId = admin.Id,
            Creator = admin
        });

        // 创意工坊集合 - 对应 workshop.html 的需求
        var postArticles = articles.Where(a => a.Type == ArticleType.Post).ToList();

        collections.Add(new Collection
        {
            Id = Guid.CreateVersion7(),
            Name = "创意工坊",
            Summary = "扎染技艺的学习与实践平台",
            Description = "提供从入门到高级的扎染教程，包含DIY指南、技法分享、现代应用案例等。让每个人都能参与到扎染艺术的创作中来。",
            Color = "#E85A4F",
            Tags = ["教程", "DIY", "学习", "实践", "创意"],
            Type = CollectionType.Article,
            ChildrenIds = postArticles.Select(a => a.Id).ToArray(),
            CreatorId = admin.Id,
            Creator = admin
        });

        dbContext.Collections.AddRange(collections);
        await dbContext.SaveChangesAsync();
        return collections;
    }

    private async Task<List<Message>> CreateMessages(List<User> users)
    {
        var messages = new List<Message>();
        var admin = users.First(u => u.UserName == "admin");
        var regularUsers = users.Where(u => u.UserRole != UserRole.Admin).ToList();

        // 系统通知消息
        foreach (var user in regularUsers)
        {
            messages.Add(new Message
            {
                Title = "欢迎加入檐下风铃",
                Content = "感谢您注册檐下风铃扎染平台，开始您的创作之旅吧！这里有丰富的扎染知识、精美的作品展示和活跃的创作社区等待您的参与。",
                Type = MessageType.System,
                Sender = admin,
                SenderId = admin.Id,
                Receiver = user,
                ReceiverId = user.Id,
                IsRead = false,
                CreatedAt = DateTimeOffset.Now.AddDays(-7)
            });

            messages.Add(new Message
            {
                Title = "平台功能介绍",
                Content = "您可以在知识库中学习扎染技艺，在纹样基因库中寻找设计灵感，在创意工坊中分享作品，在商城中购买心仪的扎染作品和材料。",
                Type = MessageType.System,
                Sender = admin,
                SenderId = admin.Id,
                Receiver = user,
                ReceiverId = user.Id,
                IsRead = user.UserName == "zhaoshewen", // 部分已读
                CreatedAt = DateTimeOffset.Now.AddDays(-6)
            });
        }

        // 订单相关消息
        var buyer = users.First(u => u.UserName == "zhaoshewen");
        var seller = users.First(u => u.UserName == "qianchuangyi");

        messages.Add(new Message
        {
            Title = "订单确认通知",
            Content = "您的订单 #ZR20240115001 已确认，商品：蓝染云纹手工围巾，金额：298元。卖家正在准备发货，请耐心等待。",
            Type = MessageType.Order,
            Sender = admin,
            SenderId = admin.Id,
            Receiver = buyer,
            ReceiverId = buyer.Id,
            IsRead = true,
            CreatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        messages.Add(new Message
        {
            Title = "您有新的订单",
            Content = "用户 赵设纹 购买了您的商品：专业扎染工具套装，订单金额：268元。请及时处理订单并安排发货。",
            Type = MessageType.Order,
            Sender = admin,
            SenderId = admin.Id,
            Receiver = seller,
            ReceiverId = seller.Id,
            IsRead = false,
            CreatedAt = DateTimeOffset.Now.AddDays(-3)
        });

        // 评论回复消息
        var commenter = users.First(u => u.UserName == "liwenchuang");
        var author = users.First(u => u.UserName == "zhaoshewen");

        messages.Add(new Message
        {
            Title = "您的作品收到新评论",
            Content = "用户 李纹创 在您的作品《云水纹的设计原理》下评论：这个渐变效果太棒了！能分享一下具体的染料配比吗？",
            Type = MessageType.Comment,
            Sender = commenter,
            SenderId = commenter.Id,
            Receiver = author,
            ReceiverId = author.Id,
            IsRead = false,
            CreatedAt = DateTimeOffset.Now.AddDays(-2)
        });

        // 关注通知
        var follower = users.First(u => u.UserName == "wangyifang");
        var followed = users.First(u => u.UserName == "zhaoshewen");

        messages.Add(new Message
        {
            Title = "新的关注者",
            Content = "用户 王艺坊 开始关注您了！他对您的扎染作品很感兴趣，快去看看他的主页吧。",
            Type = MessageType.Follow,
            Sender = follower,
            SenderId = follower.Id,
            Receiver = followed,
            ReceiverId = followed.Id,
            IsRead = false,
            CreatedAt = DateTimeOffset.Now.AddDays(-1)
        });

        // 私信对话
        var user1 = users.First(u => u.UserName == "zhaoshewen");
        var user2 = users.First(u => u.UserName == "qianchuangyi");

        messages.Add(new Message
        {
            Title = "关于扎染技法交流",
            Content = "您好！我看到您的千叠冰纹作品非常精美，想请教一下这种技法的要点。我是传统云水纹的传承人，也许我们可以互相交流学习？",
            Type = MessageType.Private,
            Sender = user1,
            SenderId = user1.Id,
            Receiver = user2,
            ReceiverId = user2.Id,
            IsRead = true,
            CreatedAt = DateTimeOffset.Now.AddHours(-12)
        });

        messages.Add(new Message
        {
            Title = "回复：关于扎染技法交流",
            Content = "非常荣幸能与您交流！千叠冰纹的关键在于折叠的层次和染料的渗透控制。我也很想学习云水纹的精髓，不如我们约个时间详细聊聊？",
            Type = MessageType.Private,
            Sender = user2,
            SenderId = user2.Id,
            Receiver = user1,
            ReceiverId = user1.Id,
            IsRead = false,
            CreatedAt = DateTimeOffset.Now.AddHours(-6)
        });

        dbContext.Messages.AddRange(messages);
        await dbContext.SaveChangesAsync();
        return messages;
    }
}