
import { memo } from 'react';
import { Flexbox } from 'react-layout-kit';
import UserAvatar from '../UserAvatar';
import { Button } from 'antd';
import { Tag } from '@lobehub/ui';
import { Github } from '@lobehub/icons';
import { config } from '../../../utils/config';
import { useActiveUser } from '../../../hooks/useActiveTabKey';


const PanelContent = memo<{ closePopover: () => void }>(({ }) => {
  const user = useActiveUser();


  function handleClick() {
    window.location.href = `https://github.com/login/oauth/authorize?client_id=${config.GithuhClientID}&redirect_uri=${location.origin}/auth/github&response_type=code`;
  }

  return (
    <Flexbox gap={2} style={{ minWidth: 200 }}>
      <Flexbox
        align={'center'}
        horizontal
        justify={'space-between'}
        style={{ padding: '6px 6px 6px 16px' }}
      >
        <Flexbox align={'center'} flex={'none'} gap={6} horizontal>
          <UserAvatar user={user} size={64} clickable />
          <div>
            <span>
              {user?.userName || "鸿钧AI"}
            </span>
            <div>
              <span>剩余次数：</span>
              <Tag color='green'>{user?.residualCredit}</Tag>
            </div>
          </div>
        </Flexbox>
      </Flexbox>
      <Flexbox style={{
        padding: '6px 16px',
      }}>
        {user?.id ? <></> : <>
          <Button 
          block
          icon={<Github />} onClick={() => {
            handleClick();
          }}>
            Github快捷登录
          </Button>

        </>}

      </Flexbox>
    </Flexbox>
  );
});

export default PanelContent;
