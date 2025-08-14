import React from 'react';
import Card from '../components/Card';
import { CodeBracketIcon } from '@heroicons/react/24/outline';

const Repositories: React.FC = () => {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Repositories</h1>
        <p className="text-gray-600">Browse and manage your code repositories</p>
      </div>
      
      <Card className="text-center py-12">
        <CodeBracketIcon className="mx-auto h-12 w-12 text-gray-400 mb-4" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">Repositories Coming Soon</h3>
        <p className="text-gray-500">This feature is under development and will be available soon.</p>
      </Card>
    </div>
  );
};

export default Repositories;
