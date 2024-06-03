import styled from "styled-components"
import { Steps, Card, Skeleton, message } from "antd"
import { useEffect, useState } from "react";
import { LoadingOutlined, LinkOutlined } from "@ant-design/icons";
import { Markdown, Tooltip } from "@lobehub/ui";
import { Search } from "../../../services/SearchService";
import { SearchResult } from "../../..";

const BodyContainer = styled.div`
    width: 100%;
    height: 100%;
    overflow: auto;
    display: flex;
`;

const MainContainer = styled.div`
    width: 100%;
    margin-left: 80px;
    margin-right: 20px;
    margin-top: 120px;
    flex: 1;
`;

const MainTitle = styled.div`
    font-size: 34px;
    font-weight: bold;
    margin-bottom: 24px;
    user-select: none;
`;

const SearchStep = styled(Steps)`
    width: 100%;
`;

const SearchContentTitle = styled.div`
    font-size: 24px;
    font-weight: bold;
    margin-bottom: 24px;
    user-select: none;
`;

const SearchContentSource = styled.span`
    float: right;
    margin-right: 24px;
    font-size: 20px;
`;

const SearchContent = styled(Markdown)`
    width: 100%;
`;

const SearchLinkContainer = styled.div`
    width: 100%;
    margin-top: 120px;
    padding: 5px;
    max-width: 300px;
    height: 80%;
`;

const RecommendContainer = styled.div`
    width: 100%;
    overflow: auto;
    display: flex;
    flex-direction: column;
    // 边框
    border: 1px solid var(--leva-colors-elevation1);
    background-color: var(--leva-colors-elevation2);
    border-radius: 8px;
    padding: 5px;
`;

const RecommendItem = styled.div`
    transition: all 0.5s;
    padding: 10px;
    cursor: pointer;
    border-radius: 8px;
    user-select: none;
    margin-bottom: 5px;

    // 鼠标悬浮时的样式
    &:hover {
        transition: all 0.5s;
        background-color: #04afff;
    }
`;

let content = '';


export default function Bing() {

    const query = new URLSearchParams(window.location.search);
    const q = query.get('q');
    const type = query.get('type');
    const [loading, setLoading] = useState(true);
    const [links, setLinks] = useState(0);
    const [step, setStep] = useState(0);
    const [value, setValue] = useState("");
    const [analysis, setAnalysis] = useState<React.ReactNode>();
    const [linkValue, setLinkValue] = useState<string>();
    const [linkVisible, setLinkVisible] = useState(false);
    // 推荐提问
    const [recommend, setRecommend] = useState<string[]>([]);
    useEffect(() => {
        handlerSearch();
    }, []);

    async function handlerSearch() {
        try {
            const response = await Search({
                query: q,
                type: type
            });
            content = '';
            for await (const result of response) {
                result?.forEach((item: any) => {
                    handleStep(item);
                });
            }
        } catch (e: any) {
            const error = JSON.parse(e.message);
            message.error(error.error, 8);
        }
    }

    function handleStep(result: SearchResult) {
        setStep(result.step);
        if (result.step === 0) {
            try {
                setAnalysis(result.content)
            } catch (e) {
                message.error("解析错误");
            }
        } else if (result.step === 1) {
            // 将字符串转换为数字
            setLinks(parseInt(result.content));
        } else if (result.step === 2) {
            setLoading(false);
            content += result.content;
            setValue(content);
        } else if (result.step === 3) {
            setLoading(false);
        } else if (result.step === -1) {
            setLinkValue(result.content);
        } else if (result.step === 4) {
            if (result.content) {
                setRecommend(JSON.parse(result.content));
            }
        } else if (result.step === 5) {
            message.info('搜索完成');
        }
    }


    return (
        <BodyContainer>
            <MainContainer>
                <MainTitle>
                    {q}
                </MainTitle>
                <SearchStep current={step}>
                    <Steps.Step
                        description={analysis}
                        icon={step === 0 ? <LoadingOutlined /> : undefined}
                        title="问题分析" />
                    <Steps.Step
                        icon={step === 1 ? <LoadingOutlined /> : undefined}
                        title={
                            step < 2 ? "等待搜索" : "搜索完成"
                        } />
                    <Steps.Step
                        icon={step === 2 ? <LoadingOutlined /> : undefined}
                        title={step < 3 ? "等待整理数据" : "数据整理完成"} />
                    <Steps.Step
                        icon={step === 3 ? <LoadingOutlined /> : undefined}
                        title={step < 4 ? "等待处理" : "处理完成"} />
                </SearchStep>
                <SearchContentTitle>
                    {
                        step === 0 ? "问题分析" :
                            step === 1 ? "搜索" :
                                step === 2 ? "数据整理" :
                                    step === 3 ? "完成" : ""
                    }

                    <Tooltip title={`来源：${links}条`}>
                        <SearchContentSource onClick={() => {
                            setLinkVisible(!linkVisible);
                        }}>
                            <LinkOutlined />
                            {links}
                        </SearchContentSource>
                    </Tooltip>
                </SearchContentTitle>
                {
                    loading ? <Skeleton active /> : <SearchContent>{value}</SearchContent>
                }

            </MainContainer>

            {
                (recommend && recommend.length > 0) && <RecommendContainer style={{
                    right: 20,
                    top: 20,
                    position: 'fixed',
                    fontSize: '12px',
                    width: '200px',
                }}
                >
                    {
                        recommend.map((item, index) => {
                            return <RecommendItem
                                onClick={() => {
                                    window.location.href = `/search?q=${item}&type=${type}`;
                                }}
                                key={index} style={{
                                    padding: '10px',
                                    cursor: 'pointer',
                                    userSelect: 'none',
                                }}>{item}</RecommendItem>
                        })
                    }
                </RecommendContainer>
            }

            <SearchLinkContainer style={{
                display: linkVisible ? 'block' : 'none'
            }}>
                {
                    linkValue && <Card style={{
                        width: '100%',
                        height: '100%',
                        overflow: 'auto',
                        display: linkVisible ? 'block' : 'none'
                    }}>
                        <Markdown>{linkValue}</Markdown>
                    </Card>
                }
            </SearchLinkContainer>
        </BodyContainer>
    )
}