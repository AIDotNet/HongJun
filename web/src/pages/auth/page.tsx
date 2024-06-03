
import { useEffect } from 'react';
import styled from 'styled-components';
import { message } from 'antd';
import { Github } from '../../services/AuthorizesService';
import { useNavigate } from 'react-router-dom';

const AuthContainer = styled.div`

    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;

`;

export default function AuthPage() {
    const navigate = useNavigate();
    function handleLogin() {
        // 判断当前是否 /auth/github
        if (window.location.pathname === '/auth/github') {
            // 获取返回的 code
            const query = new URLSearchParams(window.location.search);
            const code = query.get('code');
            // 发送请求
            if (code) {
                Github(code)
                    .then((res) => {
                        if (res.success) {
                            localStorage.setItem('token', res.data.token);
                            localStorage.setItem('role', res.data.role);
                            navigate('/home')
                        } else {
                            message.error(res.message);
                        }
                    });
            }
        }
    }

    useEffect(() => {
        handleLogin();
    }, []);

    return (
        <AuthContainer>
            <h1>
                请稍后我们正在为您登录。。。
            </h1>
        </AuthContainer>
    );
}
