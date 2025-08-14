import React from 'react';
import Card from '../Card';
import { 
  CheckCircleIcon, 
  XCircleIcon, 
  PlayIcon, 
  PlusIcon,
  UserIcon 
} from '@heroicons/react/24/outline';

const RecentActivity: React.FC = () => {
  // Mock recent activity data - in production this would come from the API
  const recentActivities = [
    {
      id: 1,
      type: 'pipeline_success',
      title: 'Pipeline succeeded',
      description: 'Build pipeline for AzureDevNexus-Platform completed successfully',
      timestamp: '2 minutes ago',
      user: 'admin',
      icon: CheckCircleIcon,
      color: 'text-green-600',
      bgColor: 'bg-green-100',
    },
    {
      id: 2,
      type: 'project_created',
      title: 'New project created',
      description: 'Sample-WebApp project was created by admin',
      timestamp: '1 hour ago',
      user: 'admin',
      icon: PlusIcon,
      color: 'text-blue-600',
      bgColor: 'bg-blue-100',
    },
    {
      id: 3,
      type: 'pipeline_started',
      title: 'Pipeline started',
      description: 'Deployment pipeline for Sample-WebApp started',
      timestamp: '2 hours ago',
      user: 'admin',
      icon: PlayIcon,
      color: 'text-blue-600',
      bgColor: 'bg-blue-100',
    },
    {
      id: 4,
      type: 'pipeline_failed',
      title: 'Pipeline failed',
      description: 'Test pipeline for AzureDevNexus-Platform failed',
      timestamp: '3 hours ago',
      user: 'admin',
      icon: XCircleIcon,
      color: 'text-red-600',
      bgColor: 'bg-red-100',
    },
    {
      id: 5,
      type: 'user_login',
      title: 'User logged in',
      description: 'admin logged into the system',
      timestamp: '4 hours ago',
      user: 'admin',
      icon: UserIcon,
      color: 'text-gray-600',
      bgColor: 'bg-gray-100',
    },
  ];

  return (
    <Card
      header={
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-gray-900">Recent Activity</h3>
        </div>
      }
    >
      <div className="flow-root">
        <ul className="-mb-8">
          {recentActivities.map((activity, activityIdx) => (
            <li key={activity.id}>
              <div className="relative pb-8">
                {activityIdx !== recentActivities.length - 1 ? (
                  <span
                    className="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200"
                    aria-hidden="true"
                  />
                ) : null}
                <div className="relative flex space-x-3">
                  <div>
                    <span className={`h-8 w-8 rounded-full flex items-center justify-center ring-8 ring-white ${activity.bgColor}`}>
                      <activity.icon className={`h-5 w-5 ${activity.color}`} aria-hidden="true" />
                    </span>
                  </div>
                  <div className="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                    <div>
                      <p className="text-sm text-gray-500">
                        {activity.title}{' '}
                        <span className="font-medium text-gray-900">
                          {activity.description}
                        </span>
                      </p>
                    </div>
                    <div className="text-right text-sm whitespace-nowrap text-gray-500">
                      <time dateTime={activity.timestamp}>{activity.timestamp}</time>
                    </div>
                  </div>
                </div>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </Card>
  );
};

export default RecentActivity;
