﻿namespace ILHotfix
{
    public class RefOutParam<T>
    {
        public RefOutParam(T value)
        {
            this.value = value;
        }

        public T value;
    }
}