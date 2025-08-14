import React from 'react';
import { Project } from '../../store/slices/projectsSlice';
import { Pipeline } from '../../store/slices/pipelinesSlice';
import Card from '../Card';
import { 
  FolderIcon, 
  CogIcon, 
  CheckCircleIcon, 
  ExclamationTriangleIcon 
} from '@heroicons/react/24/outline';

interface DashboardOverviewProps {
  projects: Project[];
  pipelines: Pipeline[];
}

const DashboardOverview: React.FC<DashboardOverviewProps> = ({ projects, pipelines }) => {
  const totalProjects = projects.length;
  const totalPipelines = pipelines.length;
  const activeProjects = projects.filter(p => p.state === 'active').length;
  const failedPipelines = pipelines.filter(p => p.lastRunResult === 'failed').length;

  const overviewCards = [
    {
      title: 'Total Projects',
      value: totalProjects,
      icon: FolderIcon,
      color: 'bg-blue-500',
      textColor: 'text-blue-600',
      bgColor: 'bg-blue-50',
    },
    {
      title: 'Active Projects',
      value: activeProjects,
      icon: CheckCircleIcon,
      color: 'bg-green-500',
      textColor: 'text-green-600',
      bgColor: 'bg-green-50',
    },
    {
      title: 'Total Pipelines',
      value: totalPipelines,
      icon: CogIcon,
      color: 'bg-purple-500',
      textColor: 'text-purple-600',
      bgColor: 'bg-purple-50',
    },
    {
      title: 'Failed Runs',
      value: failedPipelines,
      icon: ExclamationTriangleIcon,
      color: 'bg-red-500',
      textColor: 'text-red-600',
      bgColor: 'bg-red-50',
    },
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      {overviewCards.map((card) => (
        <Card key={card.title} className="hover:shadow-md transition-shadow">
          <div className="flex items-center">
            <div className={`p-3 rounded-lg ${card.bgColor}`}>
              <card.icon className={`w-6 h-6 ${card.textColor}`} />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">{card.title}</p>
              <p className="text-2xl font-bold text-gray-900">{card.value}</p>
            </div>
          </div>
        </Card>
      ))}
    </div>
  );
};

export default DashboardOverview;
