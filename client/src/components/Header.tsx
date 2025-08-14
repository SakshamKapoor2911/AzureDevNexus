import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../store';
import { User } from '../store/slices/authSlice';
import Button from './Button';
import { 
  Bars3Icon, 
  BellIcon, 
  UserCircleIcon,
  ChevronDownIcon 
} from '@heroicons/react/24/outline';

interface HeaderProps {
  onSidebarToggle: () => void;
  user: User | null;
  onLogout: () => void;
}

const Header: React.FC<HeaderProps> = ({ onSidebarToggle, user, onLogout }) => {
  const [showUserMenu, setShowUserMenu] = useState(false);
  const { notifications } = useSelector((state: RootState) => state.ui);

  const unreadNotifications = notifications.filter(n => !n.read).length;

  return (
    <header className="bg-white border-b border-gray-200 px-6 py-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center">
          <Button
            variant="ghost"
            size="sm"
            onClick={onSidebarToggle}
            className="mr-4 lg:hidden"
            icon={<Bars3Icon />}
          />
          
          <div className="flex items-center">
            <div className="w-8 h-8 bg-azure-600 rounded-lg flex items-center justify-center mr-3">
              <span className="text-white font-bold text-sm">A</span>
            </div>
            <h1 className="text-xl font-semibold text-gray-900">AzureDevNexus</h1>
          </div>
        </div>

        <div className="flex items-center space-x-4">
          {/* Notifications */}
          <div className="relative">
            <Button
              variant="ghost"
              size="sm"
              className="relative"
              icon={<BellIcon />}
            >
              {unreadNotifications > 0 && (
                <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                  {unreadNotifications > 9 ? '9+' : unreadNotifications}
                </span>
              )}
            </Button>
          </div>

          {/* User Menu */}
          <div className="relative">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setShowUserMenu(!showUserMenu)}
              className="flex items-center space-x-2"
              icon={<UserCircleIcon />}
              iconPosition="right"
            >
              <span className="hidden sm:block">{user?.username || 'User'}</span>
              <ChevronDownIcon className="w-4 h-4" />
            </Button>

            {showUserMenu && (
              <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-50 border border-gray-200">
                <div className="px-4 py-2 border-b border-gray-100">
                  <p className="text-sm font-medium text-gray-900">{user?.username}</p>
                  <p className="text-sm text-gray-500">{user?.email}</p>
                </div>
                
                <button
                  onClick={onLogout}
                  className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                >
                  Sign out
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
