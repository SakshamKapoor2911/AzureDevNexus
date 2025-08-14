import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../store';
import { fetchProjects } from '../store/slices/projectsSlice';
import Card from '../components/Card';
import Button from '../components/Button';
import LoadingSpinner from '../components/LoadingSpinner';
import { FolderIcon, MagnifyingGlassIcon, FunnelIcon } from '@heroicons/react/24/outline';

const Projects: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { projects, isLoading, error } = useSelector((state: RootState) => state.projects);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [visibilityFilter, setVisibilityFilter] = useState('all');

  useEffect(() => {
    dispatch(fetchProjects());
  }, [dispatch]);

  const filteredProjects = projects.filter(project => {
    const matchesSearch = project.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         project.description.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || project.state === statusFilter;
    const matchesVisibility = visibilityFilter === 'all' || project.visibility === visibilityFilter;
    
    return matchesSearch && matchesStatus && matchesVisibility;
  });

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

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <div className="text-red-600 mb-4">Error loading projects: {error}</div>
        <Button onClick={() => dispatch(fetchProjects())}>
          Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Projects</h1>
          <p className="text-gray-600">Manage your Azure DevOps projects</p>
        </div>
        <Button>
          New Project
        </Button>
      </div>

      {/* Filters and Search */}
      <Card>
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <div className="relative">
              <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input
                type="text"
                placeholder="Search projects..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-azure-500 focus:border-azure-500"
              />
            </div>
          </div>
          
          <div className="flex gap-2">
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-azure-500 focus:border-azure-500"
            >
              <option value="all">All Status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
              <option value="archived">Archived</option>
            </select>
            
            <select
              value={visibilityFilter}
              onChange={(e) => setVisibilityFilter(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-azure-500 focus:border-azure-500"
            >
              <option value="all">All Visibility</option>
              <option value="private">Private</option>
              <option value="public">Public</option>
            </select>
          </div>
        </div>
      </Card>

      {/* Projects Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredProjects.map((project) => (
          <Card key={project.id} className="hover:shadow-lg transition-shadow cursor-pointer">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <div className="flex items-center mb-3">
                  <div className="h-10 w-10 rounded-lg bg-azure-100 flex items-center justify-center mr-3">
                    <FolderIcon className="h-6 w-6 text-azure-600" />
                  </div>
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900">{project.name}</h3>
                    <p className="text-sm text-gray-500">{project.description}</p>
                  </div>
                </div>
                
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">Status</span>
                    <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(project.state)}`}>
                      {project.state}
                    </span>
                  </div>
                  
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">Visibility</span>
                    <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getVisibilityColor(project.visibility)}`}>
                      {project.visibility}
                    </span>
                  </div>
                  
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">Team</span>
                    <span className="text-sm text-gray-900">{project.defaultTeam.name}</span>
                  </div>
                </div>
              </div>
            </div>
            
            <div className="mt-4 pt-4 border-t border-gray-200">
              <div className="flex justify-between items-center">
                <span className="text-xs text-gray-500">
                  Updated {new Date(project.lastUpdateTime).toLocaleDateString()}
                </span>
                <Button variant="outline" size="sm">
                  View Details
                </Button>
              </div>
            </div>
          </Card>
        ))}
      </div>

      {filteredProjects.length === 0 && (
        <Card className="text-center py-12">
          <FolderIcon className="mx-auto h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No projects found</h3>
          <p className="text-gray-500 mb-4">
            {searchTerm || statusFilter !== 'all' || visibilityFilter !== 'all'
              ? 'Try adjusting your search or filters'
              : 'Get started by creating your first project'}
          </p>
          {!searchTerm && statusFilter === 'all' && visibilityFilter === 'all' && (
            <Button>Create Project</Button>
          )}
        </Card>
      )}
    </div>
  );
};

export default Projects;
