# 后端对接文档

后端在 `/openapi/v1.json` 提供了 OpenAPI 规范的接口文档。
前端可以使用相关工具生成 API 请求代码。
下面是后端的架构说明, 中间高度复用了代码, 前端也应当做好判断进行解析

## Articles

Article 用于承载文章内容, 他下面有不同的类型

```
{
    Story, // 故事
    Character, // 角色介绍
    Gene, // 基因库
    Wiki, // 百科
    Post, // 社区帖子
    Pattern // 纹样
}
```

前端的话这些是不同的板块, 但是我们需要根据不同 Type 来获取, 创建的时候前端是在不同的地方很有可能, 但是实际走的接口是这个

## Collect

这是一个独一无二的收藏, 对于每一个商品, 每一次购买都有一个唯一的编号, 类似 NFT

也有相关接口获取

## Collection

这是一个集合, 用于承载一些可以组合在一起的内容

```
CollectionType
{
    Article, // 文章
    Good, // 商品
    User // 用户
}
```

前端可以通过不同的类型来获取不同的集合内容

## Comment

可以给很多内容进行评论, 包括文章, 商品, 用户等
这里的话前端不必要关心这个评论评论给那种东西了, 因为我们都是从
这个内容本身的 ID 来获取他的评论, 所以 Comment 是没有 Type 的

## Good

就是简单的商品, 商品的话简介和一篇文章进行了关联
在前端创建的时候虽然商品信息和简介在同一个页面创建和显示, 但是后端是两个接口, 需要先创建 Article, 然后在创建 Good 的时候关联 Article


## Message

站内消息, 这个没啥可说的

## Relationship

这个用于表示两个 Entity 之间的关系

比如用户关注, 文章的喜欢, 商品的喜欢, 商品的购物车, 评论的喜欢
关系的类型是通过 `RelationshipType` 来区分的

```
{
    Unspecified = 0,
    Follow = 1,
    ArticleLike = 2,
    GoodLike = 3,
    GoodCart = 4,
    CommentLike = 5,
}
```


## Transaction

用于表示一次交易, 这种交易可以是购买商品, 也可以是用户间交易已经存在的商品 (这种商品在 Collect 中已经有了唯一编号)

在 Patch 接口中修改订单状态为 `Completed` 会自动来处理好完成交易的逻辑

## User

用户信息
