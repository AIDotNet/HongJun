
import { getInfo } from '../services/UserService';
import { SidebarTabKey } from '../store/global/initialState';
import { useEffect, useState } from 'react';

/**
 * Returns the active tab key 
 */
export const useActiveTabKey = () => {
    const pathname = window.location.pathname;
    return pathname.split('/').find(Boolean)! as SidebarTabKey;
};

/**
 * Returns the active user
 */
export const useActiveUser = () => {
    const [activeUser, setActiveUser] = useState({} as any);

    useEffect(() => {
        const fetchActiveUser = async () => {
            const user = await getInfo()
            if (user.data) {
                setActiveUser(user.data);
            }
            return null;
        };

        if (activeUser === null) {
            return;
        }

        fetchActiveUser();
    }, []);

    return activeUser;
};
