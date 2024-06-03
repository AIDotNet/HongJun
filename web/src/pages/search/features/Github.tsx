import styled from "styled-components";
import { Steps, message, Tabs, Card, Badge } from "antd";
import { useEffect, useState } from "react";
import { LoadingOutlined } from "@ant-design/icons";
import { Search } from "../../../services/SearchService";
import { SearchResult } from "../../..";
import { Markdown, Tag } from "@lobehub/ui";

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

const SearchTabs = styled(Tabs)`

`;

const Respority = styled.div`
    width: 100%;
    height: calc(100vh - 350px);
    overflow: auto;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding: 16px;
`;

const ResporityCard = styled(Card)`
    width: 100%;
    margin-bottom: 16px;

    & > h1 {
        font-size: 24px;
        font-weight: bold;
        margin-bottom: 8px;
    }
    & > p {
        font-size: 16px;
        margin-bottom: 24px;
    }
`;

const ResporityHref = styled.span`
    font-size: 16px;
    color: #1890ff;
    cursor: pointer;
    user-select: none;
    margin-right: 8px;
`;

let content = '';
export default function Github() {

    const query = new URLSearchParams(window.location.search);
    const [step, setStep] = useState(0);
    const [key, setKey] = useState('respority');
    const [querys, setQuerys] = useState<any[]>([]); // 查询数据
    const [value, setValue] = useState('' as string);
    const [respority, setRespority] = useState<any[]>(); // 仓库数据
    const type = query.get('type') || "";
    const q = query.get('q');

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
        if(result.error) {
            message.error(result.error, 8);
            return;
        }
        if (result.step === 0) {
            setQuerys(JSON.parse(result.content));
        } else if (result.step === 1) {
            setRespority(JSON.parse(result.content));
        } else if (result.step === 2) {

        } else if (result.step === 4) {
            setKey('response');
            content += result.content;
            setValue(content);
        } else if (result.step === 5) {
            message.info('搜索完成');
        }
    }

    return (
        <BodyContainer>
            <MainContainer>
                <MainTitle>
                    Github
                </MainTitle>

                <SearchStep current={step}>
                    <Steps.Step
                        description={querys?.map((item) => {
                            return (
                                <Tag>
                                    {item}
                                </Tag>
                            )
                        })}
                        icon={step === 0 ? <LoadingOutlined /> : undefined}
                        title="仓库搜索" />
                    <Steps.Step
                        icon={step === 1 ? <LoadingOutlined /> : undefined}
                        title={
                            step < 2 ? "等待仓库数据分析" : "仓库数据分析完成"
                        } />
                    <Steps.Step
                        icon={step === 2 ? <LoadingOutlined /> : undefined}
                        title={step < 3 ? "等待仓库数据整理" : "仓库数据整理完成"} />
                    <Steps.Step
                        icon={step === 3 ? <LoadingOutlined /> : undefined}
                        title={step < 4 ? "等待处理" : "处理完成"} />
                </SearchStep>
                <SearchTabs
                    defaultActiveKey={key}
                    key={key}
                    onChange={(key) => {
                        setKey(key);
                    }}
                    items={[
                        {
                            key: 'respority',
                            label: '仓库',
                            children: <Respority>
                                {
                                    respority?.map((item) => {
                                        return (
                                            <Badge.Ribbon color="pink" text={
                                                <>
                                                    Star：
                                                    {item.stargazers_count}
                                                </>
                                            }>
                                                <ResporityCard
                                                    key={item.id}
                                                    title={item.name}
                                                    extra={<>
                                                        <ResporityHref
                                                            // 打开新的页面
                                                            onClick={() => {
                                                                window.open(item.html_url, '_blank');
                                                            }}
                                                        >{item.html_url}</ResporityHref>
                                                    </>}
                                                >
                                                    {item.description}
                                                </ResporityCard>
                                            </Badge.Ribbon>
                                        )
                                    })
                                }
                            </Respority>
                        }, {
                            key: "response",
                            label: "响应",
                            children: <Markdown>
                                {value}
                            </Markdown>
                        }
                    ]}>
                </SearchTabs>
            </MainContainer>
        </BodyContainer>
    )
}