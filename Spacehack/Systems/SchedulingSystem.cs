using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spacehack.Interfaces;

namespace Spacehack
{
    class SchedulingSystem
    {
        private int _time;
        private readonly SortedDictionary<int, List<Interfaces.ISchedulable>> _scheduleables;

        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<Interfaces.ISchedulable>>();
        }

        public void Add(ISchedulable scheduleable)
        {
            int key = _time + scheduleable.Time;
            if(!_scheduleables.ContainsKey(key))
            {
                _scheduleables.Add(key, new List<ISchedulable>());
            }
            _scheduleables[key].Add(scheduleable);
        }

        public void Remove(ISchedulable scheduleable)
        {
            KeyValuePair<int, List<ISchedulable>> scheduleableListFound
              = new KeyValuePair<int, List<ISchedulable>>(-1, null);

            foreach (var scheduleablesList in _scheduleables)
            {
                if (scheduleablesList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleablesList;
                    break;
                }
            }
            if (scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if (scheduleableListFound.Value.Count <= 0)
                {
                    _scheduleables.Remove(scheduleableListFound.Key);
                }
            }
        }

        // Get the next object whose turn it is from the schedule. Advance time if necessary
        public ISchedulable Get()
        {
            var firstScheduleableGroup = _scheduleables.First();
            var firstScheduleable = firstScheduleableGroup.Value.First();
            Remove(firstScheduleable);
            _time = firstScheduleableGroup.Key;
            return firstScheduleable;
        }

        // Get the current time (turn) for the schedule
        public int GetTime()
        {
            return _time;
        }

        // Reset the time and clear out the schedule
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }
    }
}
