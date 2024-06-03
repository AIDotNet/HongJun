import { memo,  } from 'react';
import { Flexbox } from 'react-layout-kit';
import PanelContent from '../../components/User/UserPanel/PanelContent';

const UserInfo = memo(({ ...rest }: any) => {

    return (
        <Flexbox
            align={'center'}
            gap={12}
            horizontal
            justify={'space-between'}
            paddingBlock={12}
            paddingInline={12}
            {...rest}
        >  
        <PanelContent 
        closePopover={() => {

        }} />
        </Flexbox>
    );
});

export default UserInfo;
