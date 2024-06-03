
import styled from 'styled-components';
import './page.css'
import { Button } from 'antd';
import { SquareArrowRight } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GetHeat } from '../../services/SearchService';
import { Tag } from '@lobehub/ui';
import { Select } from 'antd';

const MainContianer = styled.div`
    justify-content: center;
    align-items: center;
    display: flex;
    flex-direction: column;
    height: 100%;
    width: 100%;
`;

const MainTitle = styled.div`
    font-size: 34px;
    font-weight: bold;
    color: #006dfc;
    margin-bottom: 24px;
    user-select: none;
`;

const MainDescription = styled.div`
    font-size: 16px;
    user-select: none;
`;

const MainTextarea = styled.textarea`
    width: 100%;
    height: 200px;
    padding: 16px;
    font-size: 16px;
    background-color: transparent;
    resize: none;
    outline: none;
    // 去掉所有边框
    border: none;
`;

const MainTextareContainer = styled.div`
    width: 100%;
    border: 1px solid #006dfc;
    border-radius: 8px;
    margin: 8px;
`;

const MainTextContainer = styled.div`
    margin: 24px;
    display: flex;
    width: 100%;
    max-width: 800px;
`;

const RightButton = styled(Button)`
    // 显示在右侧
    float: right;
`;

const LeftSelect = styled(Select)`
    float: left;
    width: 150px;
`;

const HeatContainer = styled.div`
    width: 100%;
    max-width: 800px;
    display: flex;
    flex-wrap: wrap;
`;

const HeatTag = styled(Tag)`
    margin: 8px;
    padding: 5px;
    font-size: 14px;
    cursor: pointer;    
`;

const QQGroup = styled.span`
    color: #006dfc;
    cursor: pointer;
    margin-left: 8px;
`;

export default function Home() {
    const [search, setSearch] = useState('');
    const [heat, setHeat] = useState([]);
    const [type, setType] = useState('bing');
    const navigate = useNavigate();

    function handleSearch() {
        if (search === '') {
            return;
        }
        navigate("/search?q=" + encodeURIComponent(search) + "&type=" + type)
    }

    function getHeat() {
        GetHeat()
            .then((data) => {
                setHeat(data);
            });
    }

    useEffect(() => {
        // 如果路由是/ 则跳转 /home
        if (window.location.pathname === '/') {
            navigate('/home');
        }
        getHeat();
    }, []);

    return (
        <MainContianer>
            <MainTitle>
                鸿钧AI搜索引擎
            </MainTitle>
            <MainDescription>
                基于.NET 8 + Semantic Kernel 实现的搜索引擎
                <QQGroup onClick={() => {
                    window.open('https://qm.qq.com/q/Ffw2MyqmZi');
                }}>
                    加入QQ群
                </QQGroup>
            </MainDescription>
            <MainTextContainer>
                <MainTextareContainer>
                    <MainTextarea
                        onKeyDown={(e) => {
                            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                                e.preventDefault(); // 阻止默认的回车键行为，如换行
                                handleSearch();
                            }
                        }}
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        maxLength={1000} />
                    <div style={{
                        display: 'flex',
                        justifyContent: 'space-between',
                        padding: '8px'
                    }}>
                        <LeftSelect value={type}
                            onChange={(value) => setType(value as string)}
                            defaultValue={type} title='选择搜索引擎'>
                            <Select.Option value="bing">必应搜索引擎</Select.Option>
                            <Select.Option value="github">Github仓库搜索</Select.Option>
                        </LeftSelect>
                        <RightButton onClick={() => handleSearch()} icon={<SquareArrowRight />} type='text'>
                        </RightButton>
                    </div>
                </MainTextareContainer>
            </MainTextContainer>
            <HeatContainer>
                {heat.map((item, index) => {
                    return (
                        <HeatTag className='tab' key={index} onClick={() => {
                            navigate("/search?q=" + encodeURIComponent(item) + "&type=" + type)
                        }}>
                            {item}
                        </HeatTag>
                    );
                })}
            </HeatContainer>
        </MainContianer>
    );
}