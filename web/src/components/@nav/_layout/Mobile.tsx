import { Search, User } from 'lucide-react';
import { Icon, MobileTabBar, MobileTabBarItemProps } from '@lobehub/ui';
import { createStyles } from 'antd-style';
import { rgba } from 'polished';
import { useNavigate } from 'react-router-dom';
import { memo } from 'react';

import { useActiveTabKey } from '../../../hooks/useActiveTabKey';
import { SidebarTabKey } from '../../../store/global/initialState';

const useStyles = createStyles(({ css, token }) => ({
  active: css`
    svg {
      fill: ${rgba(token.colorPrimary, 0.33)};
    }
  `,
  container: css`
    position: fixed;
    z-index: 100;
    right: 0;
    bottom: 0;
    left: 0;
  `,
}));

const Nav = memo(() => {
  const { styles } = useStyles();
  const activeKey = useActiveTabKey();
  const navigate = useNavigate();
  const items = [
    {
      icon: (active: any) => (
        <Icon className={active ? styles.active : undefined} icon={Search} />
      ),
      title: "搜索",
      key: SidebarTabKey.Home,
      onClick: () => {
        navigate('/home')
      }
    },
    {
      icon: (active: any) => (
        <Icon className={active ? styles.active : undefined} icon={User} />
      ),
      title: "我的",
      key: SidebarTabKey.Current,
      onClick: () => {
        navigate('/current')
      }
    },
  ] as MobileTabBarItemProps[];

  return <MobileTabBar activeKey={activeKey} className={styles.container} items={items} />;
});

Nav.displayName = 'MobileNav';

export default Nav;
