namespace HongJun.Service;

public static class PromptConstant
{

    public const string SearchUserPrompt =
        @"
使用 <data></data> 标记中的内容作为你的知识:
<data>
{0}
</data>

## 要求

- 避免提及你是从 <data></data> 获取的知识。
- 使用<data></data>中的内容作为您的知识然后再根据用户搜索的关键词，帮助用户总结数据让用户体验到搜索引擎的效果，类似于根据搜索引擎详细，并且尽量附带更多的原文内容内容然后提供多个列。
- 数据总结的内容应该是根据用户搜索的关键词，而不是固定的内容。
- 避免提及搜索关键词，尽可能的让用户体验到搜索引擎的效果。
- 您还需要作为一名专业人士，对用户搜索的关键词进行分析，然后根据搜索的关键词，帮助用户总结数据，让用户体验到搜索引擎的效果。

## 用户搜索关键词

{1}
";

    /// <summary>
    /// 根据用户提问生成相似问题
    /// </summary>
    public const string QuestionPrompt =
@"
## 要求
- 您是一个帮助用户优化提问的智能助手。
- 您需要帮助用户生成与用户提问相似的问题，不少于3个。

## 返回格式
您需要将生成的问题按照以下格式返回,它是一个json字符串数组，并且不要进行过多的解释，和回复。
[]
";

    /// <summary>
    /// 整理仓库的信息
    /// </summary>
    public const string GithubRepoPrompt = 
@"
## 要求
- 您需要根据用户提问，整理出仓库的信息，包括仓库的名称，仓库的描述，仓库的链接，并且是markdown格式。
- 你不应该解释和回复，只需要整理出仓库的信息。
";

    /// <summary>
    /// 根据提出的仓库和用户提问推荐
    /// </summary>
    public const string GithubRecommendPrompt =
@"
## 要求
- 下面我会提供一大堆GIthub的仓库，你需要根据用户提问，推荐一些仓库给用户，并且给出您的建议。
- 对于每个仓库，您需要提供仓库的名称，仓库的描述，仓库的链接，并且是markdown格式。

## 返回格式

```markdown

## [仓库名](仓库链接)
仓库描述:

建议:

```
## 用户提问
{0}

## 仓库列表
{1}

";

}