﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Data.Entity.Utilities;
using JetBrains.Annotations;
using System.Collections.ObjectModel;

namespace Microsoft.Data.Entity.Relational.Query.Expressions
{
    public class SqlFunctionExpression : Expression
    {
        private readonly List<Expression> _arguments;
        private readonly Type _returnType;

        public SqlFunctionExpression(
            [NotNull] string functionName,
            [NotNull] IEnumerable<Expression> arguments,
            [NotNull] Type returnType)
        {
            FunctionName = functionName;
            _arguments = arguments.ToList();
            _returnType = returnType;
        }

        public virtual string FunctionName { get; [param: NotNull] set; }

        public virtual IReadOnlyCollection<Expression> Arguments => _arguments;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => _returnType;

        protected override Expression Accept([NotNull] ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var specificVisitor = visitor as ISqlExpressionVisitor;

            return specificVisitor != null
                ? specificVisitor.VisitSqlFunction(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var arguments = visitor.VisitAndConvert(new ReadOnlyCollection<Expression>(_arguments), "VisitChildren");

            return arguments != Arguments
                ? new SqlFunctionExpression(FunctionName, arguments, Type)
                : this;
        }
    }
}
