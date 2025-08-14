import React from 'react';
import Card from '../components/Card';
import { DocumentTextIcon } from '@heroicons/react/24/outline';

const WorkItems: React.FC = () => {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Work Items</h1>
        <p className="text-gray-600">Manage and track your work items</p>
      </div>
      
      <Card className="text-center py-12">
        <DocumentTextIcon className="mx-auto h-12 w-12 text-gray-400 mb-4" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">Work Items Coming Soon</h3>
        <p className="text-gray-500">This feature is under development and will be available soon.</p>
      </Card>
    </div>
  );
};

export default WorkItems;
