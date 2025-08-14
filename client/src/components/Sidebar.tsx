import React from 'react';
import { NavLink } from 'react-router-dom';
import { 
  HomeIcon,
  FolderIcon,
  CogIcon,
  ChartBarIcon,
  CubeIcon,
  DocumentTextIcon,
  CodeBracketIcon
} from '@heroicons/react/24/outline';

interface SidebarProps {
  collapsed: boolean;
}

const Sidebar: React.FC<SidebarProps> = ({ collapsed }) => {
  const navigation = [
    { name: 'Dashboard', href: '/', icon: HomeIcon },
    { name: 'Projects', href: '/projects', icon: FolderIcon },
    { name: 'Pipelines', href: '/pipelines', icon: CogIcon },
    { name: 'Work Items', href: '/work-items', icon: DocumentTextIcon },
    { name: 'Repositories', href: '/repositories', icon: CodeBracketIcon },
    { name: 'Analytics', href: '/analytics', icon: ChartBarIcon },
    { name: 'Settings', href: '/settings', icon: CubeIcon },
  ];

  return (
    <div className={`bg-white border-r border-gray-200 transition-all duration-300 ${
      collapsed ? 'w-16' : 'w-64'
    }`}>
      <div className="flex flex-col h-full">
        {/* Logo */}
        <div className="flex items-center justify-center h-16 border-b border-gray-200">
          {collapsed ? (
            <div className="w-8 h-8 bg-azure-600 rounded-lg flex items-center justify-center">
              <span className="text-white font-bold text-sm">A</span>
            </div>
          ) : (
            <div className="flex items-center">
              <div className="w-8 h-8 bg-azure-600 rounded-lg flex items-center justify-center mr-3">
                <span className="text-white font-bold text-sm">A</span>
              </div>
              <span className="text-lg font-semibold text-gray-900">AzureDevNexus</span>
            </div>
          )}
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-4 py-6 space-y-2">
          {navigation.map((item) => (
            <NavLink
              key={item.name}
              to={item.href}
              className={({ isActive }) =>
                `flex items-center px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                  isActive
                    ? 'bg-azure-100 text-azure-700 border-r-2 border-azure-600'
                    : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                }`
              }
            >
              <item.icon className="w-5 h-5 flex-shrink-0" />
              {!collapsed && <span className="ml-3">{item.name}</span>}
            </NavLink>
          ))}
        </nav>

        {/* Footer */}
        {!collapsed && (
          <div className="p-4 border-t border-gray-200">
            <div className="text-xs text-gray-500 text-center">
              AzureDevNexus v1.0.0
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Sidebar;
