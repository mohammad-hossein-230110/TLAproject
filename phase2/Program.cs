using System.IO;
using System.Text;
using System.Text.Json;

#region auxiliary_classes
public class FA
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }

}
public class mystate
{
    public string Name { get; set; }
    public Dictionary<string, mystate> next_state;
    public int next_num;

    public mystate(string name)
    {
        Name = name;
        next_state = new Dictionary<string, mystate>();
    }

    public void add_neighbor(string str, mystate sta)
    {
        if (next_state.ContainsKey(str))
        {
            next_state[str] = sta;
        }
        else
        {
            next_state.Add(str, sta);
        }
    }
    public void set_next_num(string sta)
    {
        next_num = int.Parse(sta);
    }
}
public class toJson
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }

    public toJson(string input_symbol,
    Dictionary<string, Dictionary<string, string>> tran,
    string initial, List<string> final_state)
    {
        #region func_states
        var sb = new StringBuilder();
        sb.Append("{");
        foreach(var t in tran)
        {
            sb.Append($"'{t.Key}'");
            sb.Append(",");
        }
        sb.Remove(sb.Length -1,1);
        sb.Append("}");
        #endregion
        states = sb.ToString();
        input_symbols = input_symbol;
        transitions = tran;
        initial_state = initial;
        #region func_final
        sb = new StringBuilder();
        sb.Append("{");
        foreach(var t in final_state)
        {
            sb.Append($"'{t}',");
        }
        sb.Remove(sb.Length -1,1);
        sb.Append("}");
        #endregion
        final_states = sb.ToString();
    }
}
#endregion

class DFAtoSDFA
{
    #region main
    public static void Main()
    {
        var execute = initial();
        var dfa = analyze(execute);
        var sdfa_groups = dfatosdfa(dfa);
        var sdfa = minimize(sdfa_groups, dfa.Item3);
        var finish = new toJson(execute.input_symbols, sdfa.Item1,
        execute.initial_state, sdfa.Item2);
        string json = JsonSerializer.Serialize(finish);
        File.WriteAllText("myout_1.json", json);
    }

    private static (Dictionary<string,Dictionary<string,string>> , List<string>)
    minimize((IEnumerable<IGrouping<int, mystate>>, Dictionary<mystate, int>) sdfa_groups
    , List<mystate> final_state)
    {
        var list_groups = sdfa_groups.Item1.ToList();
        string[] names = new string[sdfa_groups.Item1.Count()];
        var res_dict = new Dictionary<string, Dictionary<string, string>>();
        mystate[] sdfa = new mystate[names.Length];
        for (int i =0 ; i< names.Length ; i++)
        {
            var sb =  new StringBuilder();
            foreach (var state in list_groups[i])
            {
                sb.Append(state.Name);
            }
            names[i] = sb.ToString();
            foreach (var state in list_groups[i])
            {
                state.Name = sb.ToString();
            }
        }
        for(int i = 0 ; i< list_groups.Count(); i++)
        {
            var iter_dict = new Dictionary<string, string>();
            foreach (var kvpair in list_groups[i].First().next_state)
            {
                iter_dict.Add(kvpair.Key, kvpair.Value.Name);
            }
            res_dict.Add(list_groups[i].First().Name, iter_dict);
        }
        var final = final_state.DistinctBy(x => x.Name).Select(x => x.Name).ToList();
        return (res_dict, final);
    }
    #endregion

    #region main fuction
    private static (IEnumerable<IGrouping<int, mystate>> , Dictionary<mystate, int>) 
    dfatosdfa((mystate[], List<string>, List<mystate>, mystate) t)
    {
        mystate[] newstate = reachable(t.Item4);
        newstate = newstate.OrderBy(x => Convert.ToInt32(x.Name.Last())).ToArray();
        var dict = new Dictionary<mystate, int>();
        for (int i= 0 ; i< newstate.Count() ; i++)
        {
            if (t.Item3.Contains(newstate[i]))
            {
                dict.Add(newstate[i] , 2);
            }
            else
            {
                dict.Add(newstate[i] , 1);
            }
        }
        update_state(newstate , dict);
        var groups = newstate.GroupBy(x => x.next_num);
        int x =2;
        int y = groups.Count();
        while (x != y)
        {
            x = groups.Count();
            var iter = 0 ;
            foreach (var g in groups)
            {
                iter++;
                foreach (var state in g)
                {
                    dict[state] = iter;
                }
            }
            update_state(newstate, dict);
            groups = newstate.GroupBy(x => x.next_num);
            y = groups.Count();
        }
        return (groups , dict);
    }

    private static void update_state(mystate[] newstate, Dictionary<mystate, int> dict)
    {
        foreach (var state in newstate)
        {
            var sb = new StringBuilder();
            sb.Append(dict[state]);
            foreach (var next in  state.next_state)
            {
                sb.Append(dict[next.Value]);
            }
            state.set_next_num(sb.ToString());
        }
    }

    #endregion

    #region auxiliary fuctions(check_final_state & myname)
    private static bool check_final_state(List<mystate> list_next_theta,
    List<mystate> final)
    {
        bool res = false;
        foreach (var f in final)
        {
            if(list_next_theta.Contains(f))
            {
                res = true;
                return res;
            }
        }
        return res;
    }

    private static string myname(List<mystate> list_next_theta)
    {
        if (list_next_theta.Count == 0)
        {
            return "TRAP";
        }
        var sb = new StringBuilder();
        var llist = list_next_theta.OrderBy(x => Convert.ToInt32(x.Name.Last()));
        foreach(var l in llist)
        {
            sb.Append(l.Name);
        }
        return sb.ToString();
    }
    #endregion

    #region auxiliary functions(mystr_1_2_3 & landatrans)
    private static List<string> mystr_1(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str);
        sb.Remove(str.Length - 1, 1);
        sb.Remove(0, 1);
        return sb.ToString().Split(",").Select(x => $"{x[1]}{x[2]}").ToList();
    }
    private static List<string> mystr_2(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str);
        sb.Remove(str.Length - 1, 1);
        sb.Remove(0, 1);
        return sb.ToString().Split(",").Select(x => $"{x[1]}").ToList();
    }
    private static mystate[] mystr_3(Dictionary<string, mystate> all,
    Dictionary<string, Dictionary<string, string>> transitions)
    {
        mystate[] res = new mystate[all.Count];
        var iter = transitions.ToList();
        for (int i = 0; i < iter.Count; i++)
        {
            var inner_dict = iter[i].Value.ToList();
            for (int j = 0; j < inner_dict.Count; j++)
            {
                var trans = inner_dict[j].Key;
                var state_str = inner_dict[j].Value;
                all[iter[i].Key].add_neighbor(trans, all[state_str]);
            }
            res[i] = all[iter[i].Key];
        }
        return res;
    }
    #endregion

    #region auxiliary fuctions(landatrans, combine)
    private static mystate[] reachable(mystate start)
    {
        List<mystate> stack = new List<mystate>();
        List<mystate> res = new List<mystate>();
        stack.Add(start);
        while (stack.Count != 0)
        {
            var iter = stack.First();
            foreach (var n in iter.next_state)
            {
                if (!stack.Contains(n.Value) && !res.Contains(n.Value))
                {
                    stack.Add(n.Value);
                }
            }
            if (!res.Contains(iter))
            {
                res.Add(iter);
            }
            stack.Remove(iter);
        }
        return res.ToArray();
    }
    private static void combine(mystate state_1, mystate state_2)
    {
        var sb = new StringBuilder();
        if (Convert.ToInt32(state_1.Name.Last()) > Convert.ToInt32(state_2.Name.Last()))
        {
            sb.Append(state_2.Name);
            sb.Append(state_1.Name);
        }
        else
        {
            sb.Append(state_1.Name);
            sb.Append(state_2.Name);
        }
        state_1.Name = sb.ToString();
    }
    #endregion

    #region initial&analyze functions

    private static FA initial()
    {
        string text =
        File.ReadAllText("C:\\git\\TLA01-Projects\\samples\\phase2-sample\\in\\input1.json");
        FA test = JsonSerializer.Deserialize<FA>(text);
        return test;
    }
    
    private static (mystate[], List<string>, List<mystate>, mystate)
    analyze(FA execute)
    {
        Dictionary<string, mystate> all = new Dictionary<string, mystate>();
        List<mystate> final_state = new List<mystate>();
        mystate start;
        List<string> n_states = mystr_1(execute.states);
        List<string> n_final = mystr_1(execute.final_states);
        List<string> n_symbols = mystr_2(execute.input_symbols);
        for (int i = 0; i < n_states.Count; i++)
        {
            all.Add(n_states[i], new mystate(n_states[i]));
        }
        for (int i = 0; i < n_final.Count; i++)
        {
            final_state.Add(all[n_final[i]]);
        }
        start = all[execute.initial_state];
        mystate[] res = mystr_3(all, execute.transitions);
        return (res, n_symbols, final_state, start);
    }
    #endregion
}