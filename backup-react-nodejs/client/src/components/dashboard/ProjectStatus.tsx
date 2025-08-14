import React from 'react';
import { Project } from '../../store/slices/projectsSlice';
import Card from '../Card';
import { FolderIcon, EyeIcon } from '@heroicons/react/24/outline';
import { Link } from 'react-router-dom';

interface ProjectStatusProps {
  projects: Project[];
}

const ProjectStatus: React.FC<ProjectStatusProps> = ({ projects }) => {
  const getStatusColor = (state: string) => {
    switch (state.toLowerCase()) {
      case 'active':
        return 'bg-green-100 text-green-800';
      case 'inactive':
        return 'bg-gray-100 text-gray-800';
      case 'archived':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getVisibilityColor = (visibility: string) => {
    switch (visibility.toLowerCase()) {
      case 'private':
        return 'bg-red-100 text-red-800';
      case 'public':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <Card
      header={
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-gray-900">Project Status</h3>
          <Link
            to="/projects"
            className="text-sm text-azure-600 hover:text-azure-700 font-medium"
          >
            View All
          </Link>
        </div>
      }
    >
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Project
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Visibility
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Team
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {projects.slice(0, 5).map((project) => (
              <tr key={project.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="flex items-center">
                    <div className="flex-shrink-0 h-8 w-8">
                      <div className="h-8 w-8 rounded-lg bg-azure-100 flex items-center justify-center">
                        <FolderIcon className="h-5 w-5 text-azure-600" />
                      </div>
                    </div>
                    <div className="ml-4">
                      <div className="text-sm font-medium text-gray-900">
                        {project.name}
                      </div>
                      <div className="text-sm text-gray-500">
                        {project.description}
                      </div>
                    </div>
                  </div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(project.state)}`}>
                    {project.state}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getVisibilityColor(project.visibility)}`}>
                    {project.visibility}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {project.defaultTeam.name}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <Link
                    to={`/projects/${project.id}`}
                    className="text-azure-600 hover:text-azure-900 flex items-center"
                  >
                    <EyeIcon className="h-4 w-4 mr-1" />
                    View
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      
      {projects.length === 0 && (
        <div className="text-center py-8">
          <FolderIcon className="mx-auto h-12 w-12 text-gray-400" />
          <h3 className="mt-2 text-sm font-medium text-gray-900">No projects</h3>
          <p className="mt-1 text-sm text-gray-500">Get started by creating a new project.</p>
        </div>
      )}
    </Card>
  );
};

export default ProjectStatus;
