import { ActionIcon } from '@lobehub/ui';
import { Search } from 'lucide-react';
import { memo } from 'react';
import { SidebarTabKey } from '../../../../store/global/initialState';
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';


export interface TopActionProps {
    tab?: SidebarTabKey;
}

const TopActions = memo<TopActionProps>(({ tab }) => {

    const navigate = useNavigate();
    const items = [
        {
            href: '/home',
            icon: Search,
            title: "搜索",
            enable: true,
            key: SidebarTabKey.Home,
            onClick: () => {
                navigate('/home')
            }
        },
    ];

    return (
        <>
            {items
                .filter((item: any) => !item.disabled)
                .map((item: any) => {
                    return (
                        <Link
                            to={item.href}
                            onClick={(e) => {
                                e.preventDefault();
                                item.onClick();
                            }}
                        >
                            <ActionIcon
                                active={tab === item?.key}
                                icon={item.icon}
                                placement={'right'}
                                size="large"
                                title={item.title}
                            />
                        </Link>)
                })}
        </>
    );
});

export default TopActions;
